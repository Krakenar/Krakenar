import { ErrorCodes, StatusCodes, type ApiFailure, type ProblemDetails } from "@/types/api";

export function isError(e: unknown, statusCode?: StatusCodes, errorCode?: ErrorCodes): boolean {
  try {
    const failure = e as ApiFailure;
    const details = failure.data as ProblemDetails;
    if (!details || !details.error) {
      return false;
    }
    if (statusCode && statusCode !== failure.status) {
      return false;
    }
    if (errorCode && errorCode !== details.error.code) {
      return false;
    }
    return true;
  } catch (e: unknown) {
    console.error(e);
    return false;
  }
}
