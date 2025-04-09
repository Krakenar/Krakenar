using Bogus;

namespace Krakenar.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class PersonNameTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "BuildFullNameString: it should return null when the list is empty.")]
  public void BuildFullNameString_it_should_return_null_when_the_list_is_empty()
  {
    Assert.Null(PersonName.BuildFullName(Array.Empty<string>()));
  }

  [Fact(DisplayName = "BuildFullNameString: it should return null when the list only contains empty names.")]
  public void BuildFullNameString_it_should_return_null_when_the_list_only_contains_empty_names()
  {
    Assert.Null(PersonName.BuildFullName(["", "   "]));
  }

  [Fact(DisplayName = "BuildFullNameString: it should build the full name of a person.")]
  public void BuildFullNameString_it_should_build_the_full_name_of_a_person()
  {
    string[] names = [_faker.Name.FirstName(), $"  {_faker.Name.FirstName()}  ", _faker.Name.LastName()];
    string expected = string.Join(' ', names.Select(name => name.Trim()));
    Assert.Equal(expected, PersonName.BuildFullName(names));
  }

  [Fact(DisplayName = "BuildFullNameUnit: it should return null when the list is empty.")]
  public void BuildFullNameUnit_it_should_return_null_when_the_list_is_empty()
  {
    Assert.Null(PersonName.BuildFullName(Array.Empty<PersonName>()));
  }

  [Fact(DisplayName = "BuildFullNameUnit: it should build the full name of a person.")]
  public void BuildFullNameUnit_it_should_build_the_full_name_of_a_person()
  {
    string[] names = [_faker.Name.FirstName(), $"  {_faker.Name.FirstName()}  ", _faker.Name.LastName()];
    string expected = string.Join(' ', names.Select(name => name.Trim()));
    Assert.Equal(expected, PersonName.BuildFullName(names.Select(name => new PersonName(name)).ToArray()));
  }

  [Theory(DisplayName = "ctor: it should create a new person name.")]
  [InlineData("PersonName")]
  [InlineData("  This is a person name.  ")]
  public void ctor_it_should_create_a_new_person_name(string value)
  {
    PersonName personName = new(value);
    Assert.Equal(value.Trim(), personName.Value);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException when the value is empty.")]
  [InlineData("")]
  [InlineData("  ")]
  public void ctor_it_should_throw_ValidationException_when_the_value_is_empty(string value)
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new PersonName(value));
    Assert.All(exception.Errors, e =>
    {
      Assert.Equal("Value", e.PropertyName);
      Assert.Equal("NotEmptyValidator", e.ErrorCode);
    });
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when the value is too long.")]
  public void ctor_it_should_throw_ValidationException_when_the_value_is_too_long()
  {
    string value = _faker.Random.String(PersonName.MaximumLength + 1);

    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new PersonName(value));
    Assert.All(exception.Errors, e =>
    {
      Assert.Equal("MaximumLengthValidator", e.ErrorCode);
      Assert.Equal("Value", e.PropertyName);
    });
  }

  [Theory(DisplayName = "TryCreate: it should return a person name when the value is not empty.")]
  [InlineData("PersonName")]
  [InlineData("  This is a person name.  ")]
  public void TryCreate_it_should_return_a_person_name_when_the_value_is_not_empty(string value)
  {
    PersonName? personName = PersonName.TryCreate(value);
    Assert.NotNull(personName);
    Assert.Equal(value.Trim(), personName.Value);
  }

  [Theory(DisplayName = "TryCreate: it should return null when the value is null or whitespace.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void TryCreate_it_should_return_null_when_the_value_is_null_or_whitespace(string? value)
  {
    Assert.Null(PersonName.TryCreate(value));
  }
}
