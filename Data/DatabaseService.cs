using System;
using System.Data.SQLite;
using System.IO;
using Dapper;
using TaskManagerApp.Models;
using System.Collections.Generic;
using System.Linq;
using Task = TaskManagerApp.Models.Task;

namespace TaskManagerApp.Data
{
    public class DatabaseService
    {
        private const string DatabaseName = "TaskManager.db"; 
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = $"Data Source={DatabaseName};Version=3;";
            EnsureDatabaseCreated(); 
        }

        
        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        
        private void EnsureDatabaseCreated()
        {
            
            if (!File.Exists(DatabaseName))
            {
                
                SQLiteConnection.CreateFile(DatabaseName);
            }

            using (var connection = GetConnection())
            {
                connection.Open();

                
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Login TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        Email TEXT
                    );
                ");

               
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Projects (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT
                    );
                ");

                
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        Priority TEXT,
                        DueDate DATETIME,
                        Status TEXT,
                        ProjectId INTEGER,
                        AssigneeId INTEGER,
                        FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
                        FOREIGN KEY (AssigneeId) REFERENCES Users(Id)
                    );
                ");

                
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Comments (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        TaskId INTEGER,
                        UserId INTEGER,
                        Text TEXT,
                        CreatedAt DATETIME,
                        FOREIGN KEY (TaskId) REFERENCES Tasks(Id),
                        FOREIGN KEY (UserId) REFERENCES Users(Id)
                    );
                ");


                
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS UserRoles (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        RoleName TEXT NOT NULL UNIQUE
                    );
                ");

                
                AddDefaultRoles(connection);

                
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS UserProjects (
                        UserId INTEGER,
                        ProjectId INTEGER,
                         FOREIGN KEY (UserId) REFERENCES Users(Id),
                          FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
                         PRIMARY KEY (UserId, ProjectId)
                    );
                ");

                
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS UserUserRoles (
                        UserId INTEGER,
                        RoleId INTEGER,
                        FOREIGN KEY (UserId) REFERENCES Users(Id),
                         FOREIGN KEY (RoleId) REFERENCES UserRoles(Id),
                           PRIMARY KEY (UserId, RoleId)
                    );
                ");

                
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Teams (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Description TEXT
                    );
                ");
                
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS UserTeams (
                        UserId INTEGER,
                        TeamId INTEGER,
                        FOREIGN KEY (UserId) REFERENCES Users(Id),
                         FOREIGN KEY (TeamId) REFERENCES Teams(Id),
                            PRIMARY KEY (UserId, TeamId)
                    );
                ");
                
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS TaskTeams (
                        TaskId INTEGER,
                        TeamId INTEGER,
                         FOREIGN KEY (TaskId) REFERENCES Tasks(Id),
                        FOREIGN KEY (TeamId) REFERENCES Teams(Id),
                        PRIMARY KEY (TaskId, TeamId)
                    );
                ");
            }
        }

        private void AddDefaultRoles(SQLiteConnection connection)
        {
            
            int count = connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM UserRoles");
            if (count == 0) 
            {
                
                connection.Execute("INSERT INTO UserRoles (RoleName) VALUES (@RoleName)", new { RoleName = "Администратор" });
                connection.Execute("INSERT INTO UserRoles (RoleName) VALUES (@RoleName)", new { RoleName = "Пользователь" });
            }

        }

       

        public User GetUserByLogin(string login)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Login = @Login", new { Login = login });
            }
        }

        public User GetUserById(int userId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = userId });
            }
        }

        public void UpdateUser(User user)
        {
            using (var connection = GetConnection())
            {
                connection.Execute("UPDATE Users SET Login = @Login, PasswordHash = @PasswordHash, Email = @Email WHERE Id = @Id", user);
            }
        }

        public int CreateUser(User user)
        {
            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<int>(
                     "INSERT INTO Users (Login, PasswordHash, Email) VALUES (@Login, @PasswordHash, @Email); SELECT last_insert_rowid();",
                    user);
            }
        }

        public int CreateTask(Task task)
        {
            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<int>(
                     "INSERT INTO Tasks (Title, Description, Priority, DueDate, Status, ProjectId, AssigneeId) " +
                     "VALUES (@Title, @Description, @Priority, @DueDate, @Status, @ProjectId, @AssigneeId); SELECT last_insert_rowid();",
                     task);
            }
        }
        public int CreateProject(Project project)
        {
            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<int>(
                    "INSERT INTO Projects (Title, Description) VALUES (@Title, @Description); SELECT last_insert_rowid();",
                    project);
            }
        }

        public void DeleteProject(int projectId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute("DELETE FROM Projects WHERE Id = @Id", new { Id = projectId });
            }
        }


        public List<Project> GetAllProjects()
        {
            using (var connection = GetConnection())
            {
                return connection.Query<Project>("SELECT * FROM Projects").AsList();
            }
        }


        public Project GetProjectById(int projectId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryFirstOrDefault<Project>("SELECT * FROM Projects WHERE Id = @Id", new { Id = projectId });
            }
        }


        public List<Task> GetAllTasks()
        {
            using (var connection = GetConnection())
            {
                return connection.Query<Task>("SELECT * FROM Tasks").AsList();
            }
        }
        public Task GetTaskById(int taskId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryFirstOrDefault<Task>("SELECT * FROM Tasks WHERE Id = @Id", new { Id = taskId });
            }
        }
        public List<Task> GetTasksByUserId(int userId)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<Task>("SELECT * FROM Tasks WHERE AssigneeId = @AssigneeId", new { AssigneeId = userId }).AsList();
            }
        }

        public List<Task> GetTasksByProjectId(int projectId)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<Task>("SELECT * FROM Tasks WHERE ProjectId = @ProjectId", new { ProjectId = projectId }).AsList();
            }
        }

        public int CreateComment(Comment comment)
        {
            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<int>("INSERT INTO Comments (TaskId, UserId, Text, CreatedAt) VALUES (@TaskId, @UserId, @Text, @CreatedAt); SELECT last_insert_rowid();", comment);
            }
        }

        public List<Comment> GetCommentsByTaskId(int taskId)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<Comment>("SELECT * FROM Comments WHERE TaskId = @TaskId", new { TaskId = taskId }).AsList();
            }
        }
        public List<Comment> GetAllComments()
        {
            using (var connection = GetConnection())
            {
                return connection.Query<Comment>("SELECT * FROM Comments").AsList();
            }
        }

        public void UpdateTask(Task task)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(
                   "UPDATE Tasks SET Title = @Title, Description = @Description, Priority = @Priority, DueDate = @DueDate, Status = @Status, ProjectId = @ProjectId, AssigneeId = @AssigneeId WHERE Id = @Id", task);
            }
        }
        public void DeleteTask(int taskId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(
                   "DELETE FROM Tasks WHERE Id = @Id", new { Id = taskId });
            }
        }

        public void DeleteComment(int commentId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute("DELETE FROM Comments WHERE Id = @Id", new { Id = commentId });
            }
        }
        

        public int CreateUserRole(string roleName)
        {
            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<int>(
                    "INSERT INTO UserRoles (RoleName) VALUES (@RoleName); SELECT last_insert_rowid();",
                    new { RoleName = roleName });
            }
        }

        public List<string> GetAllUserRoles()
        {
            using (var connection = GetConnection())
            {
                return connection.Query<string>("SELECT RoleName FROM UserRoles").ToList();
            }
        }

        public int GetUserRoleIdByName(string roleName)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryFirstOrDefault<int>("SELECT Id FROM UserRoles WHERE RoleName = @RoleName", new { RoleName = roleName });
            }
        }

        
        public void AddUserToProject(int userId, int projectId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute("INSERT INTO UserProjects (UserId, ProjectId) VALUES (@UserId, @ProjectId)", new { UserId = userId, ProjectId = projectId });
            }
        }

        
        public List<Project> GetUserProjects(int userId)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<Project>(@"
            SELECT p.* FROM Projects p
            INNER JOIN UserProjects up ON p.Id = up.ProjectId
            WHERE up.UserId = @UserId", new { UserId = userId }).ToList();
            }
        }
        
        public void AddRoleToUser(int userId, int roleId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute("INSERT INTO UserUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)", new { UserId = userId, RoleId = roleId });
            }
        }
        
        public List<string> GetUserRoles(int userId)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<string>(@"
                    SELECT r.RoleName FROM UserRoles r
                    INNER JOIN UserUserRoles uur ON r.Id = uur.RoleId
                    WHERE uur.UserId = @UserId", new { UserId = userId }).ToList();
            }
        }

        public List<User> GetAllUsers()
        {
            using (var connection = GetConnection())
            {
                return connection.Query<User>("SELECT * FROM Users").AsList();
            }
        }

        
        public int CreateTeam(Team team)
        {
            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<int>(
                    "INSERT INTO Teams (Name, Description) VALUES (@Name, @Description); SELECT last_insert_rowid();",
                    team);
            }
        }
        public List<Team> GetAllTeams()
        {
            using (var connection = GetConnection())
            {
                return connection.Query<Team>("SELECT * FROM Teams").AsList();
            }
        }
        public Team GetTeamById(int teamId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryFirstOrDefault<Team>("SELECT * FROM Teams WHERE Id = @Id", new { Id = teamId });
            }
        }
        public void DeleteTeam(int teamId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute("DELETE FROM Teams WHERE Id = @Id", new { Id = teamId });
            }
        }

       
        public void AddUserToTeam(int userId, int teamId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute("INSERT INTO UserTeams (UserId, TeamId) VALUES (@UserId, @TeamId)", new { UserId = userId, TeamId = teamId });
            }
        }

        
        public List<User> GetTeamUsers(int teamId)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<User>(@"
                    SELECT u.* FROM Users u
                    INNER JOIN UserTeams ut ON u.Id = ut.UserId
                    WHERE ut.TeamId = @TeamId", new { TeamId = teamId }).ToList();
            }
        }
        
        public List<Team> GetUserTeams(int userId)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<Team>(@"
                    SELECT t.* FROM Teams t
                    INNER JOIN UserTeams ut ON t.Id = ut.TeamId
                    WHERE ut.UserId = @UserId", new { UserId = userId }).ToList();
            }
        }
        
        public void AddTaskToTeam(int taskId, int teamId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute("INSERT INTO TaskTeams (TaskId, TeamId) VALUES (@TaskId, @TeamId)", new { TaskId = taskId, TeamId = teamId });
            }
        }
        
        public List<Task> GetTeamTasks(int teamId)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<Task>(@"
                   SELECT t.* FROM Tasks t
                   INNER JOIN TaskTeams tt ON t.Id = tt.TaskId
                   WHERE tt.TeamId = @TeamId", new { TeamId = teamId }).ToList();
            }
        }
    }
}