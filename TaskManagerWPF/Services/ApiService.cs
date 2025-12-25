// ApiService.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskManagerWPF.Models;

namespace TaskManagerWPF.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private string? _currentToken;

        public ApiService(string baseUrl = "http://fin.gitbebra.ru:24578/")
        {
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetToken(string token)
        {
            _currentToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearToken()
        {
            _currentToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<User> RegisterAsync(UserRegister registerData)
        {
            return await PostAsync<UserRegister, User>("/auth/register", registerData);
        }


        public async Task<TokenResponse> LoginAsync(string username, string password)
        {
            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = username,
                ["password"] = password
            };

            var content = new FormUrlEncodedContent(formData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync("/auth/login", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenResponse>(json, _jsonOptions)!;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            return await GetAsync<User>("/auth/me");
        }

        public async Task<List<Project>> GetProjectsAsync(int skip = 0, int limit = 100)
        {
            return await GetAsync<List<Project>>($"/projects/?skip={skip}&limit={limit}");
        }

        public async Task<Project> CreateProjectAsync(ProjectCreate projectData)
        {
            return await PostAsync<ProjectCreate, Project>("/projects/", projectData);
        }

        public async Task<Project> UpdateProjectAsync(int projectId, ProjectCreate projectData)
        {
            return await PutAsync<ProjectCreate, Project>($"/projects/{projectId}", projectData);
        }

        public async Task DeleteProjectAsync(int projectId)
        {
            await DeleteAsync($"/projects/{projectId}");
        }

        public async Task<List<TaskWithDetails>> GetTasksAsync(
            int? projectId = null,
            int? statusId = null,
            int? priorityId = null,
            int skip = 0,
            int limit = 100)
        {
            var query = $"/tasks/?skip={skip}&limit={limit}";
            if (projectId.HasValue) query += $"&project_id={projectId}";
            if (statusId.HasValue) query += $"&status_id={statusId}";
            if (priorityId.HasValue) query += $"&priority_id={priorityId}";

            return await GetAsync<List<TaskWithDetails>>(query);
        }

        public async Task<TaskItem> CreateTaskAsync(TaskCreate taskData)
        {
            return await PostAsync<TaskCreate, TaskItem>("/tasks/", taskData);
        }

        public async Task<TaskWithDetails> GetTaskAsync(int taskId)
        {
            return await GetAsync<TaskWithDetails>($"/tasks/{taskId}");
        }

        public async Task<TaskItem> UpdateTaskAsync(int taskId, TaskCreate taskData)
        {
            return await PutAsync<TaskCreate, TaskItem>($"/tasks/{taskId}", taskData);
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            await DeleteAsync($"/tasks/{taskId}");
        }

        public async Task<List<Priority>> GetPrioritiesAsync()
        {
            return await GetAsync<List<Priority>>("/priorities/");
        }

        public async Task<List<Status>> GetStatusesAsync()
        {
            return await GetAsync<List<Status>>("/statuses/");
        }

        public async Task<List<Comment>> GetTaskCommentsAsync(int taskId)
        {
            return await GetAsync<List<Comment>>($"/comments/task/{taskId}");
        }

        public async Task<Comment> CreateCommentAsync(CommentCreate commentData)
        {
            return await PostAsync<CommentCreate, Comment>("/comments/", commentData);
        }

        private async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions)!;
        }

        private async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions)!;
        }

        private async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions)!;
        }

        private async Task DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }
    }
}