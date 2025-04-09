namespace Krakenar.Infrastructure;

public class DatabaseProviderNotSupportedException : NotSupportedException
{
  public DatabaseProvider DatabaseProvider
  {
    get => (DatabaseProvider)Data[nameof(DatabaseProvider)]!;
    private set => Data[nameof(DatabaseProvider)] = value;
  }

  public DatabaseProviderNotSupportedException(DatabaseProvider databaseProvider)
    : base($"The database provider '{databaseProvider}' is not supported.")
  {
    DatabaseProvider = databaseProvider;
  }
}
