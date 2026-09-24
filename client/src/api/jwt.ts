// The backend embeds the role via `new Claim(ClaimTypes.Role, role)` written directly through
// the JwtSecurityToken constructor, which does not apply ASP.NET's short-claim-name outbound
// mapping. So the payload key is the long claim URI, not "role" - check both to be safe.
const ROLE_CLAIM_URI = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

function decodeJwtPayload(token: string): Record<string, unknown> | null {
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const json = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join(''),
    )
    return JSON.parse(json)
  } catch {
    return null
  }
}

export function getRoleFromToken(token: string): string | null {
  const payload = decodeJwtPayload(token)
  if (!payload) return null

  const role = payload[ROLE_CLAIM_URI] ?? payload['role']
  return typeof role === 'string' ? role : null
}
