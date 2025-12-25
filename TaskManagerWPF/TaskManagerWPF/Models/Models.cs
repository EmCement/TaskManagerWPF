using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TaskManagerWPF.Models
{
    public class ValidationError
    {
        public List<object> Loc { get; set; } = new();  // Может быть string или int
        public string Msg { get; set; } = null!;
        public string Type { get; set; } = null!;
    }

    public class HTTPValidationError
    {
        public List<ValidationError> Detail { get; set; } = new();
    }

    public class UserRegister
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FullName { get; set; }
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null!;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = null!;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "bearer";
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string Role { get; set; } = "user";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProjectCreate
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class Priority
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Level { get; set; }
        public string Color { get; set; } = "#6B7280";
    }

    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int OrderNum { get; set; }
        public bool IsFinal { get; set; }
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int ProjectId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedById { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TaskWithDetails : TaskItem
    {
        public Priority? Priority { get; set; }
        public Status? Status { get; set; }
        public Project? Project { get; set; }
    }

    public class TaskCreate
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("due_date")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("project_id")]
        public int ProjectId { get; set; }

        [JsonPropertyName("priority_id")]
        public int? PriorityId { get; set; }

        [JsonPropertyName("status_id")]
        public int? StatusId { get; set; }

        [JsonPropertyName("assignee_ids")]
        public List<int>? AssigneeIds { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public int TaskId { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CommentCreate
    {
        public string Content { get; set; } = null!;
        public int TaskId { get; set; }
    }
}