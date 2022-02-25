using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using MinecraftServerlist.Services.Abstractions;
using System.Globalization;

namespace MinecraftServerlist.Services.Implementation;

public sealed class ServerMonitoringService : IServerMonitoringService, IDisposable
{
    private readonly InfluxDBClient _influxDbClient;
    private readonly WriteOptions _writeOptions;

    private const string InfluxOrganization = "mcbrowser";
    private const string InfluxPlayerBucket = "players";
    private const string InfluxPlayerMeasurement = "players";

    // Second-based write precision is enough for the player statistics
    private const WritePrecision InfluxWritePrecision = WritePrecision.S;

    public ServerMonitoringService(InfluxDBClient influxDbClient)
    {
        _influxDbClient = influxDbClient;
        _writeOptions = WriteOptions.CreateNew()
            .MaxRetries(24)
            .Build();
    }

    public void StoreServerStatistic(int serverId, bool online, int onlinePlayers, DateTime timestamp)
    {
        using var writeApi = _influxDbClient.GetWriteApi(_writeOptions);

        var point = PointData.Measurement(InfluxPlayerMeasurement)
            .Tag("server_id", $"{serverId}")
            .Field("online", online)
            .Field("online_players", onlinePlayers)
            .Timestamp(timestamp, InfluxWritePrecision);

        writeApi.WriteMeasurement(InfluxPlayerBucket, InfluxOrganization, InfluxWritePrecision, point);
    }

    private async Task GetServerStatistics(int serverId, DateTime? startDateTime, DateTime? stopDateTime)
    {
        var startQueryTime = "0";
        var stopQueryTime = "now()";

        if (startDateTime is { } notNullStartDateTime)
        {
            startQueryTime = notNullStartDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
        }

        if (stopDateTime is { } notNullStopDateTime)
        {
            stopQueryTime = notNullStopDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
        }

        var fluxQuery = $"from(bucket:\"{InfluxPlayerBucket}\") |> range(start: {startQueryTime}, stop: {stopQueryTime})";
        var fluxTables = await _influxDbClient.GetQueryApi().QueryAsync(fluxQuery, InfluxOrganization);
        foreach (var fluxTable in fluxTables)
        {
            var fluxRecords = fluxTable.Records;
            foreach (var fluxRecord in fluxRecords)
            {
            }
        }
    }

    public void Dispose()
    {
        _influxDbClient.Dispose();
    }
}