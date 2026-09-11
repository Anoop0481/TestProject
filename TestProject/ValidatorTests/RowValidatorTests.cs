using Xunit;

namespace TestProject.Tests.ValidatorTests;

public class RowValidatorTests
{
    [Fact]
    public void Supplier_MissingSupplierName_ReturnsError()
    {
        var row = new Dictionary<string, string> { ["SupplierId"] = "S1" };

        var errors = RowValidator.Validate(row, "Supplier");

        Assert.Contains("Missing required field: SupplierName", errors);
    }

    [Fact]
    public void Supplier_AllFieldsPresent_ReturnsNoErrors()
    {
        var row = new Dictionary<string, string>
        {
            ["SupplierId"] = "S1",
            ["SupplierName"] = "Acme Corp"
        };

        var errors = RowValidator.Validate(row, "Supplier");

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData("0", true)]
    [InlineData("-5", true)]
    [InlineData("abc", true)]
    [InlineData("100.50", false)]
    public void CustomerTransaction_AmountValidation(string amount, bool expectError)
    {
        var row = new Dictionary<string, string>
        {
            ["TransactionId"] = "T1",
            ["CustomerId"] = "C1",
            ["Amount"] = amount
        };

        var errors = RowValidator.Validate(row, "CustomerTransaction");

        Assert.Equal(expectError, errors.Any(e => e.Contains("Amount")));
    }

    [Fact]
    public void Employee_InvalidEmail_ReturnsError()
    {
        var row = new Dictionary<string, string>
        {
            ["EmployeeId"] = "E1",
            ["Email"] = "not-an-email"
        };

        var errors = RowValidator.Validate(row, "Employee");

        Assert.Contains("Email format is invalid", errors);
    }
}