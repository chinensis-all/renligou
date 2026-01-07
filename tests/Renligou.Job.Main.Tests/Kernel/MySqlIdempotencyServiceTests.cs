using System.Data;
using System.Data.Common;
using Renligou.Core.Infrastructure.Data.Connections;
using Renligou.Core.Infrastructure.Data.Inbox;

namespace Renligou.Job.Main.Tests.Kernel;

[TestFixture]
public class MySqlIdempotencyServiceTests
{
    private MockDbConnectionFactory _factory;
    private MySqlIdempotencyService _service;

    [SetUp]
    public void SetUp()
    {
        _factory = new MockDbConnectionFactory();
        _service = new MySqlIdempotencyService(_factory);
    }

    [Test]
    public async Task AlreadyProcessedAsync_WhenExists_ShouldReturnTrue()
    {
        // Arrange
        _factory.Connection.ScalarResult = 1;

        // Act
        var result = await _service.AlreadyProcessedAsync("msg-1", CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task AlreadyProcessedAsync_WhenNotExists_ShouldReturnFalse()
    {
        // Arrange
        _factory.Connection.ScalarResult = 0;

        // Act
        var result = await _service.AlreadyProcessedAsync("msg-1", CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task MarkProcessedAsync_ShouldExecuteSuccessfully()
    {
        // Act
        await _service.MarkProcessedAsync("msg-1", CancellationToken.None);

        // Assert
        // If no exception, it succeeded in our mock
        Assert.Pass();
    }

    #region Mock Classes

    private class MockDbConnectionFactory : IDbConnectionFactory
    {
        public MockDbConnection Connection { get; } = new();
        public DbConnection Create() => Connection;
    }

    // A very simple mock connection that can handle Dapper's basic needs
    private class MockDbConnection : DbConnection
    {
        public object? ScalarResult { get; set; }
        
        public override string ConnectionString { get; set; } = "";
        public override string Database => "";
        public override string DataSource => "";
        public override string ServerVersion => "";
        public override ConnectionState State => ConnectionState.Open;

        public override void ChangeDatabase(string databaseName) { }
        public override void Close() { }
        public override void Open() { }

        protected override DbCommand CreateDbCommand()
        {
            return new MockDbCommand(this);
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }
    }

    private class MockDbCommand : DbCommand
    {
        private readonly MockDbConnection _connection;
        public MockDbCommand(MockDbConnection connection) => _connection = connection;

        public override string CommandText { get; set; } = "";
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection? DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection => new MockParameterCollection();
        protected override DbTransaction? DbTransaction { get; set; }

        public override void Cancel() { }
        public override int ExecuteNonQuery() => 0;
        public override object? ExecuteScalar() => _connection.ScalarResult;
        public override void Prepare() { }
        protected override DbParameter CreateDbParameter() => new MockParameter();
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotImplementedException();
        
        public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_connection.ScalarResult);
        }
        
        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }

    private class MockParameterCollection : DbParameterCollection
    {
        private readonly List<object> _list = new();
        public override int Count => _list.Count;
        public override object SyncRoot => _list;
        public override int Add(object value) { _list.Add(value); return _list.Count - 1; }
        public override void AddRange(Array values) { foreach(var v in values) _list.Add(v!); }
        public override void Clear() => _list.Clear();
        public override bool Contains(object value) => _list.Contains(value);
        public override bool Contains(string value) => false;
        public override void CopyTo(Array array, int index) { }
        public override System.Collections.IEnumerator GetEnumerator() => _list.GetEnumerator();
        public override int IndexOf(object value) => _list.IndexOf(value);
        public override int IndexOf(string parameterName) => -1;
        public override void Insert(int index, object value) => _list.Insert(index, value);
        public override void Remove(object value) => _list.Remove(value);
        public override void RemoveAt(int index) => _list.RemoveAt(index);
        public override void RemoveAt(string parameterName) { }
        protected override DbParameter GetParameter(int index) => (DbParameter)_list[index];
        protected override DbParameter GetParameter(string parameterName) => throw new NotImplementedException();
        protected override void SetParameter(int index, DbParameter value) => _list[index] = value;
        protected override void SetParameter(string parameterName, DbParameter value) => throw new NotImplementedException();
    }

    private class MockParameter : DbParameter
    {
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; } = "";
        public override int Size { get; set; }
        public override string SourceColumn { get; set; } = "";
        public override bool SourceColumnNullMapping { get; set; }
        public override object? Value { get; set; }
        public override void ResetDbType() { }
    }

    #endregion
}
