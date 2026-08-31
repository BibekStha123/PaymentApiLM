using System.Linq;
using Xunit;

public class Class1Tests
{
	private readonly ITestOutputHelper _output;

	public Class1Tests(ITestOutputHelper output)
	{
		_output = output;
	}

	[Fact]
	public void GetLargePayments_FiltersAndSortsDescending()
	{
		var sut = new Class1();
		var amounts = new decimal[] { 50m, 150m, 75m, 300m, 120m };

		var result = sut.GetLargePayments(amounts).ToList();

		_output.WriteLine("Large payments: " + string.Join(", ", result));

		Assert.Equal(new decimal[] { 300m, 150m, 120m }, result);
	}
}
