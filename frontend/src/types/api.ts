export type ApiError = {
  code: string;
  message: string;
  data: Record<string, unknown>;
};

export type ApiFailure = {
  data?: unknown;
  status: number;
};

export type ApiResult<T> = {
  data: T;
  status: number;
};

export type ApiVersion = {
  title: string;
  version: string;
};

export type ProblemDetails = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  error?: ApiError;
};
