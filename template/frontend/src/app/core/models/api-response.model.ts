export interface ValidationError {
  error: string;
  detail: string;
}

export interface ApiResponse {
  success: boolean;
  message: string;
  errors: ValidationError[];
}

export interface ApiResponseWithData<T> extends ApiResponse {
  data: T;
}

export interface PaginatedResponse<T> extends ApiResponse {
  data: T[];
  currentPage: number;
  totalPages: number;
  totalCount: number;
}

export interface PaginatedList<T> {
  data: T[];
  totalItems: number;
  currentPage: number;
  totalPages: number;
}
