﻿using Krakenar.Contracts.Fields;

namespace Krakenar.Contracts.Contents;

public record SaveContentLocalePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldValuePayload> FieldValues { get; set; } = [];

  public SaveContentLocalePayload() : this(string.Empty)
  {
  }

  public SaveContentLocalePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
