using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using WebUI.Controllers;

namespace WebUI.Controllers
{
    public class WorkspaceController : BaseController
    {
        private readonly IRepositoryService<Workspace> _workspaceRepository;

        public WorkspaceController(IRepositoryService<Workspace> workspaceRepository)
        {
            _workspaceRepository = workspaceRepository;
        }

        // GET: Workspace
        public async Task<IActionResult> Index()
        {
            return View(await _workspaceRepository.FindAsync(x => true));
        }

        // GET: Workspace/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workspace = await _workspaceRepository.FindOneAsync(x => x.Id == id);
            if (workspace == null)
            {
                return NotFound();
            }

            return View(workspace);
        }

        // GET: Workspace/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workspace/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,UserId")] Workspace workspace)
        {
            if (ModelState.IsValid)
            {
                await _workspaceRepository.InsertOneAsync(workspace);
                return RedirectToAction(nameof(Index));
            }
            return View(workspace);
        }

        // GET: Workspace/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workspace = await _workspaceRepository.FindOneAsync(x => x.Id == id);
            if (workspace == null)
            {
                return NotFound();
            }
            return View(workspace);
        }

        // POST: Workspace/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Description,UserId")] Workspace workspace)
        {
            if (id != workspace.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dbWorkspace = await _workspaceRepository.FindOneAsync(x => x.Id == id);
                    if (dbWorkspace == null)
                    {
                        return NotFound();
                    }

                    dbWorkspace.Title = workspace.Title;
                    dbWorkspace.Description = workspace.Description;
                    dbWorkspace.UserId = workspace.UserId;

                    await _workspaceRepository.ReplaceOneAsync(x => x.Id == id, workspace);
                }
                catch (Exception)
                {
                    if (!WorkspaceExists(workspace.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workspace);
        }

        // GET: Workspace/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workspace = await _workspaceRepository.FindOneAsync(x => x.Id == id);
            if (workspace == null)
            {
                return NotFound();
            }

            return View(workspace);
        }

        // POST: Workspace/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var workspace = await _workspaceRepository.FindOneAsync(x => x.Id == id);
            if (workspace != null)
            {
                await _workspaceRepository.DeleteOneAsync(x => x.Id == id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool WorkspaceExists(string id)
        {
            return _workspaceRepository.FindOneAsync(x => x.Id == id) != null;
        }
    }
}
