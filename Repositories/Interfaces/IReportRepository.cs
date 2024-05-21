﻿namespace FireEscape.Repositories.Interfaces;

public interface IReportRepository
{
    Task<string> CreateReportAsync(Order order, Protocol protocol, string fileName);
}
