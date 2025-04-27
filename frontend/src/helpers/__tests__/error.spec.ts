import { describe, expect, it, test } from "vitest";

import { ErrorCodes, StatusCodes, type ApiError, type ApiFailure, type ProblemDetails } from "@/types/api";
import { isError } from "../error";

describe("isError", () => {
  test.each(["test", 0.0, 0n, false, [], null, undefined, { status: 401 } as ApiFailure])("should return false when there is no problem details", (e) => {
    expect(isError(e)).toBe(false);
  });

  it.concurrent("should return false when there is no error", () => {
    const failure: ApiFailure = {
      status: StatusCodes.BadRequest,
      data: {
        title: "Bad Request",
        status: StatusCodes.BadRequest,
      } as ProblemDetails,
    };
    expect(isError(failure)).toBe(false);
  });

  it.concurrent("should return false when the status code does not match", () => {
    const failure: ApiFailure = {
      status: StatusCodes.Conflict,
      data: {
        title: "Unique Slug Already Used",
        status: StatusCodes.Conflict,
        error: { code: ErrorCodes.UniqueSlugAlreadyUsed } as ApiError,
      } as ProblemDetails,
    };
    expect(isError(failure, StatusCodes.BadRequest)).toBe(false);
  });

  it.concurrent("should return false when the error code does not match", () => {
    const failure: ApiFailure = {
      status: StatusCodes.Conflict,
      data: {
        title: "Unique Slug Already Used",
        status: StatusCodes.Conflict,
        error: { code: ErrorCodes.UniqueSlugAlreadyUsed } as ApiError,
      } as ProblemDetails,
    };
    expect(isError(failure, undefined, ErrorCodes.InvalidCredentials)).toBe(false);
  });

  it.concurrent("should not match the status code when it is not provided", () => {
    const failure: ApiFailure = {
      status: StatusCodes.BadRequest,
      data: {
        title: "Invalid Credentials",
        status: StatusCodes.BadRequest,
        error: { code: ErrorCodes.InvalidCredentials } as ApiError,
      } as ProblemDetails,
    };
    expect(isError(failure, undefined, ErrorCodes.InvalidCredentials)).toBe(true);
  });

  it.concurrent("should not match the error code when it is not provided", () => {
    const failure: ApiFailure = {
      status: StatusCodes.BadRequest,
      data: {
        title: "Invalid Credentials",
        status: StatusCodes.BadRequest,
        error: { code: ErrorCodes.InvalidCredentials } as ApiError,
      } as ProblemDetails,
    };
    expect(isError(failure, StatusCodes.BadRequest)).toBe(true);
  });

  it.concurrent("should return true when the status code and the error code match", () => {
    const failure: ApiFailure = {
      status: StatusCodes.BadRequest,
      data: {
        title: "Invalid Credentials",
        status: StatusCodes.BadRequest,
        error: { code: ErrorCodes.InvalidCredentials } as ApiError,
      } as ProblemDetails,
    };
    expect(isError(failure, StatusCodes.BadRequest, ErrorCodes.InvalidCredentials)).toBe(true);
  });
});
