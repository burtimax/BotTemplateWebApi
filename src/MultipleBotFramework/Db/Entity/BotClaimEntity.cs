﻿using Microsoft.EntityFrameworkCore;

namespace MultipleBotFramework.Db.Entity;

/// <summary>
/// Разрешения.
/// </summary>
[Comment("Таблица разрешений.")]
public class BotClaimEntity : BaseEntity<long>
{
    /// <summary>
    /// Имя разрешения.
    /// </summary>
    [Comment("Имя разрешения.")]
    public string Name { get; set; }

    /// <summary>
    /// Описание разрешения.
    /// </summary>
    [Comment("Описание разрешения.")]
    public string? Description { get; set; }
}