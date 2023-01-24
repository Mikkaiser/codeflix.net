using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{

    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
    {
        _categoryTestFixture = categoryTestFixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        //Arrange
        //Applying the Fixture Pattern
        var validCategory = _categoryTestFixture.GetValidCategory();
        var datetimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var datetimeAfter = DateTime.Now;

        //Assert

        //Refactoring - fluent assertions
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));

        Assert.True(category.CreatedAt > datetimeBefore);
        Assert.True(category.CreatedAt < datetimeAfter);
        Assert.True(category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool IsActive)
    {
        //Arrange
        //Using fixture
        var validCategory = _categoryTestFixture.GetValidCategory();

        var datetimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, IsActive);
        var datetimeAfter = DateTime.Now;

        //Assert
        Assert.NotNull(category);
        Assert.Equal(validCategory.Name, category.Name);
        Assert.Equal(validCategory.Description, category.Description);
        Assert.NotEqual(default(Guid), category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt > datetimeBefore);
        Assert.True(category.CreatedAt < datetimeAfter);
        Assert.Equal(category.IsActive, IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        DomainEntity.Category validCategory = _categoryTestFixture.GetValidCategory();

        Action action = () => new DomainEntity.Category(name!, validCategory.Description);
        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should not be empty or null", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        Action action = () => new DomainEntity.Category("Category Name", null!);
        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Description should not be empty or null", exception.Message);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThanThreeCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("ab")]
    public void InstantiateErrorWhenNameIsLessThanThreeCharacters(string invalidName)
    {
        Action action = () => new DomainEntity.Category(invalidName, "Category Ok Description");
        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should be at least three characters long", exception.Message);
    }


    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThanTwoHundredFiftyFiveCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThanTwoHundredFiftyFiveCharacters()
    {
        string invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        Action action = () => new DomainEntity.Category(invalidName, "Category Ok Description");
        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should have less or the same as 255 characters long", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThanTenThousandCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThanTenThousandCharacters()
    {
        string invalidDescription = String.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());

        Action action = () => new DomainEntity.Category("Valid Name", invalidDescription);
        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Description should have less or the same as ten thousand characters long", exception.Message);
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var validData = new
        {
            Name = "Category Name",
            Description = "Category Description",
        };

        var category = new DomainEntity.Category(validData.Name, validData.Description, false);
        category.Activate();

        Assert.True(category.IsActive);
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var validData = new
        {
            Name = "Category Name",
            Description = "Category Description",
        };

        var category = new DomainEntity.Category(validData.Name, validData.Description, true);

        category.Deactivate();
        Assert.False(category.IsActive);

    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        DomainEntity.Category category = new DomainEntity.Category("Category Name", "Category Description");
        var newValues = new { Name = "NewName", Description = "New Description" };

        category.Update(newValues.Name, newValues.Description);

        Assert.Equal(newValues.Name, category.Name);
        Assert.Equal(newValues.Description, category.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        DomainEntity.Category category = new DomainEntity.Category("Category Name", "Category Description");
        var newValues = new { Name = "NewName"};
        string currentDescription = category.Description;

        category.Update(newValues.Name);

        Assert.Equal(newValues.Name, category.Name);
        Assert.Equal(currentDescription, category.Description);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmptyOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsEmptyOnUpdate(string? name)
    {
        DomainEntity.Category category = new DomainEntity.Category("Category Name", "Category Description");

        Action action = () => category.Update(name!);
        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should not be empty or null", exception.Message);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThanThreeCharactersOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("ab")]
    public void InstantiateErrorWhenNameIsLessThanThreeCharactersOnUpdate(string invalidName)
    {
        DomainEntity.Category category = new("Category Name", "Category Description");

        Action action = () => category.Update(invalidName!);
        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should be at least three characters long", exception.Message);
    }


    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThanTwoHundredFiftyFiveCharactersOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThanTwoHundredFiftyFiveCharactersOnUpdate()
    {
        DomainEntity.Category category = new("Category Name", "Category Description");
        string invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        Action action = () => category.Update(invalidName!);
        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should have less or the same as 255 characters long", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThanTenThousandCharactersOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThanTenThousandCharactersOnUpdate()
    {
        DomainEntity.Category category = new("Category Name", "Category Description");
        string invalidDescription = String.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());

        Action action = () => category.Update("Category New Name", invalidDescription);
        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Description should have less or the same as ten thousand characters long", exception.Message);
    }
}

