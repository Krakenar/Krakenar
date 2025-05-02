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

export enum ErrorCodes {
  CustomIdentifierAlreadyUsed = "CustomIdentifierAlreadyUsed",
  IncorrectUserPassword = "IncorrectUserPassword",
  InvalidCredentials = "InvalidCredentials",
  LanguageAlreadyUsed = "LanguageAlreadyUsed",
  LocaleAlreadyUsed = "LocaleAlreadyUsed",
  UniqueNameAlreadyUsed = "UniqueNameAlreadyUsed",
  UniqueSlugAlreadyUsed = "UniqueSlugAlreadyUsed",
  UserHasNoPassword = "UserHasNoPassword",
  UserIsDisabled = "UserIsDisabled",
  UserNotFound = "UserNotFound",
}

export type ProblemDetails = {
  type?: string | null;
  title?: string | null;
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  error?: ApiError | null;
};

export enum StatusCodes {
  BadRequest = 400,
  Conflict = 409,
  NotFound = 404,
}
