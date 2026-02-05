export interface TaskStatus {
  Succeeded: boolean;
  Message: string;
  BrokenRules: string[];
  StatusCode: number;
  data: {
    id: number;
    lookupCategoryId: number;
    nameEn: string;
    nameAr: string;
    code: string;
  }
}