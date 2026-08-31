# API Gateway

`PaymentDetailApi.Gateway` is a [YARP](https://github.com/microsoft/reverse-proxy) (Yet Another Reverse Proxy) based API gateway. It is the single entry point clients talk to; it authenticates, authorizes, rate-limits, and CORS-checks every request, then forwards it to one of several load-balanced backend instances of the `API` project.

```
Client -> https://localhost:7186 (Gateway) -> https://localhost:7128 | 7129 | 7130 (API instances)
```

Clients never address the backend instances directly — only the Gateway's port.

## Request pipeline (`Program.cs`)

Requests pass through middleware in this order:

1. **Exception handler** — catches unhandled exceptions from anything below it and returns a generic `500` JSON error instead of leaking a stack trace.
2. **CORS** (`UseCors`) — only `http://localhost:4200` is allowed, any header/method.
3. **Authentication** (`UseAuthentication`) — validates the JWT bearer token (issuer, audience, lifetime, signing key) against the `Jwt` settings in `appsettings.json`.
4. **Authorization** (`UseAuthorization`) — enforces the `Authenticated` policy on routes that require it.
5. **Rate limiter** (`UseRateLimiter`) — a sliding-window limiter named `sliding` (10 requests per 30s window, 3 segments).
6. **Upstream-failure logger** — a custom inline middleware that, after `next()` runs, checks if the response is `502`/`503`/`504` and logs + rewrites it into a JSON `"Service Unavailable"` body. This is what surfaces when a chosen backend destination is down or unreachable.
7. **`MapReverseProxy()`** — YARP's own middleware, which does the actual route matching and proxying to a backend destination. This is the terminal step.

## Routing config (`appsettings.json` → `ReverseProxy`)

YARP config has two parts: **Routes** (match incoming requests) and **Clusters** (pools of backend destinations).

### Routes

| Route | Path match | Cluster | Auth | Rate limit |
|---|---|---|---|---|
| `registerRoute` | `POST /api/v1/register` | `paymentDetailApiCluster` | — | — |
| `loginRoute` | `POST /api/v1/login` | `paymentDetailApiCluster` | — | — |
| `paymentDetailsRoute` | `/api/v1/payment-details/{**catch-all}` | `paymentDetailApiCluster` | `Authenticated` | `sliding` |
| `paymentDetailApiRoute` | `/{**catch-all}` (fallback) | `paymentDetailApiCluster` | `Authenticated` | — |

All routes currently point at the same cluster, so today this config is mainly about applying different auth/rate-limit policies per path — not about splitting traffic across different services.

### Cluster: `paymentDetailApiCluster`

```json
{
  "LoadBalancingPolicy": "RoundRobin",
  "HealthCheck": {
    "Active": {
      "Enabled": true,
      "Interval": "00:00:10",
      "Timeout": "00:00:05",
      "Policy": "ConsecutiveFailures",
      "Path": "/health"
    }
  },
  "Metadata": { "ConsecutiveFailuresHealthPolicy.Threshold": "2" },
  "Destinations": {
    "paymentDetailApiDestination1": { "Address": "https://localhost:7128" },
    "paymentDetailApiDestination2": { "Address": "https://localhost:7129" },
    "paymentDetailApiDestination3": { "Address": "https://localhost:7130" }
  }
}
```

- **Destinations** — three instances of the same `API` project, each started via a different `launchSettings.json` profile (`https`, `https-instance2`, `https-instance3`), listening on 7128/7129/7130 respectively.
- **Load balancing** — `RoundRobin` picks one destination per request, cycling through the healthy ones. It is *not* a broadcast — each request goes to exactly one instance.
- **Active health checks** — every 10 seconds, YARP itself calls `GET /health` on each destination (the API exposes this via `app.MapGet("/health", () => Results.Ok("Healthy"))` in `API/Program.cs`). A 5s timeout applies per probe.
- **Failure policy** — `ConsecutiveFailures` with `Threshold: 2` marks a destination unhealthy after 2 consecutive failed probes, removing it from the round-robin rotation until it starts passing health checks again.

## Example: end-to-end trace

`GET https://localhost:7186/api/v1/payment-details/123` with a valid JWT:

1. Gateway receives the request on port 7186.
2. Auth middleware validates the JWT; authorization middleware checks the `Authenticated` policy.
3. Rate limiter checks the `sliding` window for this client.
4. YARP matches `paymentDetailsRoute` (`/api/v1/payment-details/{**catch-all}`) → resolves to `paymentDetailApiCluster`.
5. `RoundRobin` picks a currently-healthy destination, e.g. `https://localhost:7129`.
6. YARP forwards the request to `https://localhost:7129/api/v1/payment-details/123` and streams the response back to the client.
7. If that destination had returned `502/503/504`, step 6's response middleware would log it and return a uniform `"Service Unavailable"` JSON body instead.

## Running locally with multiple destinations

By default only one API instance (the `https` profile, port 7128) may be running, which is why health probes to 7129/7130 fail (connection refused) until those are also started. To exercise real load balancing, run all three profiles from the `API` project in separate terminals:

```
dotnet run --launch-profile https
dotnet run --launch-profile https-instance2
dotnet run --launch-profile https-instance3
```

Then run the Gateway (`dotnet run` from `PaymentDetailApi.Gateway`) and send requests to `https://localhost:7186`.
