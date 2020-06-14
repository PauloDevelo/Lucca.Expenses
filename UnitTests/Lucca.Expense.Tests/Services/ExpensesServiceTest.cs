using FluentAssertions;
using FluentAssertions.Extensions;
using Lucca.Expense.DTOModels;
using Lucca.Expense.EFModels;
using Lucca.Expense.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lucca.Expense.Tests.Services
{
    public class ExpensesServiceTest: IDisposable
    {
        private readonly ServiceProvider serviceProvider;
        private readonly IExpensesService expensesService;
        private readonly DateTime currentTime;

        private readonly UserInfo natashaUser;
        private readonly UserInfo anthonyUser;


        public ExpensesServiceTest()
        {
            this.currentTime = 14.June(2020).AddHours(11).AddMinutes(42).AsUtc();
            var timeServiceMock = new Mock<ITimeService>();
            timeServiceMock.Setup(timeService => timeService.GetCurrentUtcTime()).Returns(this.currentTime);

            var services = new ServiceCollection();
            services.AddDbContext<LuccaContext>(options => options.UseInMemoryDatabase(databaseName: "UnitTestInMemoryDatabase"), ServiceLifetime.Transient);
            this.serviceProvider = services.BuildServiceProvider();

            var luccaContext = serviceProvider.GetService<LuccaContext>();
            natashaUser = new UserInfo() { Id = Guid.Parse("C95F9FEB-31A4-46AD-B3AC-6FEC5804863F"), Currency = "RUB", FirstName = "Natasha", LastName = "Romanova" };
            anthonyUser = new UserInfo() { Id = Guid.Parse("32952B48-7D36-48C4-9413-F9E70B002C16"), Currency = "USD", FirstName = "Anthony", LastName = "Stark" };
            luccaContext.UserInfo.Add(natashaUser);
            luccaContext.UserInfo.Add(anthonyUser);
            luccaContext.SaveChanges();

            this.expensesService = new ExpensesService(serviceProvider, timeServiceMock.Object);
        }

        [Fact]
        public async Task AddExpense_WhenWellConfigured_ShouldNotThrowAnException()
        {
            // Arrange
            var dtoExpense = new DTOExpense() { Amount = (decimal)1457.23, Category = "Restaurant", Comment = "A comment", Currency = "USD", PurchasedOn = this.currentTime.AddDays(-1), UserId = anthonyUser.Id };
            
            // Act
            await this.expensesService.AddExpense(dtoExpense);

            // Assert
            var expenses = await this.expensesService.GetExpenses(new ExpensesQuery());
            expenses.Should().HaveCount(1);
        }

        [Fact]
        public async Task AddExpense_WhenPurchaseDateInFuture_ShouldThrowAnExceptionWithTheCorrectErrorCode()
        {
            // Arrange
            var dtoExpense = new DTOExpense() { Amount = (decimal)1.23, Category = "Restaurant", Comment = "A comment", Currency = "USD", PurchasedOn = this.currentTime.AddDays(1), UserId = anthonyUser.Id };

            // Act
            Func<Task> addExpense = () => this.expensesService.AddExpense(dtoExpense);

            // Assert
            ExpenseValidationException ex = await Assert.ThrowsAsync<ExpenseValidationException>(addExpense);

            var errorCodes = ex.GetErrorCodes();
            errorCodes.Should().HaveCount(1);
            errorCodes.First().Should().Be(ExpenseErrorCode.PurchaseInFuture);
        }

        [Fact]
        public async Task AddExpense_WhenPurchaseMoreThan3MonthAgo_ShouldThrowAnExceptionWithTheCorrectErrorCode()
        {
            // Arrange
            var dtoExpense = new DTOExpense() { Amount = (decimal)10.23, Category = "Restaurant", Comment = "A comment", Currency = "USD", PurchasedOn = this.currentTime.AddDays(-100), UserId = anthonyUser.Id };

            // Act
            Func<Task> addExpense = () => this.expensesService.AddExpense(dtoExpense);

            // Assert
            ExpenseValidationException ex = await Assert.ThrowsAsync<ExpenseValidationException>(addExpense);

            var errorCodes = ex.GetErrorCodes();
            errorCodes.Should().HaveCount(1);
            errorCodes.First().Should().Be(ExpenseErrorCode.PurchaseMoreThan3MonthOld);
        }

        [Fact]
        public async Task AddExpense_WhenCommentIsMissing_ShouldThrowAnExceptionWithTheCorrectErrorCode()
        {
            // Arrange
            var dtoExpense = new DTOExpense() { Amount = (decimal)121.23, Category = "Restaurant", Comment = "", Currency = "USD", PurchasedOn = this.currentTime.AddDays(-1), UserId = anthonyUser.Id };

            // Act
            Func<Task> addExpense = () => this.expensesService.AddExpense(dtoExpense);

            // Assert
            ExpenseValidationException ex = await Assert.ThrowsAsync<ExpenseValidationException>(addExpense);

            var errorCodes = ex.GetErrorCodes();
            errorCodes.Should().HaveCount(1);
            errorCodes.First().Should().Be(ExpenseErrorCode.CommentIsMandatory);
        }

        [Fact]
        public async Task AddExpense_WhenAddingDuplicatedExpense_ShouldThrowAnExceptionWithTheCorrectErrorCode()
        {
            // Arrange
            var dtoExpense = new DTOExpense() { Amount = (decimal)1457.23, Category = "Restaurant", Comment = "A comment", Currency = "USD", PurchasedOn = this.currentTime.AddDays(-1), UserId = anthonyUser.Id };
            await this.expensesService.AddExpense(dtoExpense);

            // Act
            Func<Task> addExpense = () => this.expensesService.AddExpense(dtoExpense);

            // Assert
            ExpenseValidationException ex = await Assert.ThrowsAsync<ExpenseValidationException>(addExpense);

            var errorCodes = ex.GetErrorCodes();
            errorCodes.Should().HaveCount(1);
            errorCodes.First().Should().Be(ExpenseErrorCode.DuplicatedExpense);
        }

        [Fact]
        public async Task AddExpense_WhenAddingDuplicatedExpenseWithoutComment_ShouldThrowAnExceptionWithTheCorrectErrorCode()
        {
            // Arrange
            var dtoExpense1 = new DTOExpense() { Amount = (decimal)1457.23, Category = "Restaurant", Comment = "A comment", Currency = "USD", PurchasedOn = this.currentTime.AddDays(-1), UserId = anthonyUser.Id };
            await this.expensesService.AddExpense(dtoExpense1);

            var dtoExpense2 = new DTOExpense() { Amount = (decimal)1457.23, Category = "Restaurant", Comment = "", Currency = "USD", PurchasedOn = this.currentTime.AddDays(-1), UserId = anthonyUser.Id };
            
            // Act
            Func<Task> addExpense = () => this.expensesService.AddExpense(dtoExpense2);

            // Assert
            ExpenseValidationException ex = await Assert.ThrowsAsync<ExpenseValidationException>(addExpense);

            var errorCodes = ex.GetErrorCodes();
            errorCodes.Should().HaveCount(2);
            errorCodes.Should().Contain(ExpenseErrorCode.DuplicatedExpense);
            errorCodes.Should().Contain(ExpenseErrorCode.CommentIsMandatory);
        }

        [Fact]
        public async Task AddExpense_WhenExpenseCurrencyDifferentFromUserCurrency_ShouldThrowAnExceptionWithTheCorrectErrorCode()
        {
            // Arrange
            var dtoExpense = new DTOExpense() { Amount = (decimal)1457.23, Category = "Restaurant", Comment = "A comment", Currency = "EUR", PurchasedOn = this.currentTime.AddDays(-1), UserId = anthonyUser.Id };
            
            // Act
            Func<Task> addExpense = () => this.expensesService.AddExpense(dtoExpense);

            // Assert
            ExpenseValidationException ex = await Assert.ThrowsAsync<ExpenseValidationException>(addExpense);

            var errorCodes = ex.GetErrorCodes();
            errorCodes.Should().HaveCount(1);
            errorCodes.First().Should().Be(ExpenseErrorCode.ExpenseCurrencyDifferentThanUserCurrency);
        }

        public void Dispose()
        {
            var luccaContext = this.serviceProvider.GetService<LuccaContext>();

            var expenses = luccaContext.Expenses.AsQueryable();
            expenses.ForEachAsync((expense) => luccaContext.Expenses.Remove(expense));

            var users = luccaContext.UserInfo.AsQueryable();
            users.ForEachAsync((user) => luccaContext.UserInfo.Remove(user));

            luccaContext.SaveChanges();
        }
    }
}
