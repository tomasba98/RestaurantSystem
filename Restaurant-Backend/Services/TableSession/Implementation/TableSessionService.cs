﻿using Restaurant_Backend.Services.DataAccessLayer;

namespace Restaurant_Backend.Services.TableSession.Implementation;

using Microsoft.EntityFrameworkCore;
using Restaurant_Backend.Entities;

public class TableSessionService : ITableSessionService
{
    private readonly IGenericService<TableSession> _tableGenericService;

    public TableSessionService (IGenericService<TableSession> tableGenericService)
    {
        _tableGenericService = tableGenericService;
    }
    public async Task<TableSession> StartSessionAsync(TableSession tableSession)
    {
        await _tableGenericService.InsertAsync(tableSession);
        return tableSession;
    }
    public async Task CloseSessionAsync(Guid tableSessionId)
    {
        TableSession session = await _tableGenericService.GetByIdAsync(tableSessionId) ?? throw new InvalidOperationException("TableSession not found.");
        session.EndTime = DateTime.UtcNow;
        await _tableGenericService.UpdateAsync(session);
    }
    public async Task<bool> HasActiveSessionAsync(Guid tableId)
    {
        return await _tableGenericService
            .FilterByExpressionLinq(s => s.TableId == tableId && s.EndTime == null)
            .AnyAsync();
    }

    public async Task<IEnumerable<TableSession>> GetSessionsByTableIdAsync(Guid tableId)
    {
        return await _tableGenericService
                .FilterByExpressionLinq(session => session.TableId == tableId)
                .OrderByDescending(session => session.StartTime)
                .ToListAsync();
    }

    public async Task<IEnumerable<TableSession>> GetAllActiveSessionsAsync()
    {
        return await _tableGenericService
            .FilterByExpressionLinq(session => session.EndTime == null)
            .ToListAsync();
    }
}
