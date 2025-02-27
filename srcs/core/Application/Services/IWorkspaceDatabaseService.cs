namespace Application.Services;

public interface IWorkspaceDatabaseService {
	Task CreateWorkspaceDatabaseAsync(string id);
}