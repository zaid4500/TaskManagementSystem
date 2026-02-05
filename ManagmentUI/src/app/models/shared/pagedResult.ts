export interface PagedResult<T> {
  Succeeded: boolean;
  Message: string;
  BrokenRules: string[];
  StatusCode: number;
  data: {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
  }
}