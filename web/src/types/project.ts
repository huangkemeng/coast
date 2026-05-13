export interface Project {
  id: number;
  name: string;
  description: string | null;
  requirementCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface ProjectListItem {
  id: number;
  name: string;
  description: string | null;
  requirementCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProjectRequest {
  name: string;
  description?: string;
}

export interface UpdateProjectRequest extends CreateProjectRequest {}