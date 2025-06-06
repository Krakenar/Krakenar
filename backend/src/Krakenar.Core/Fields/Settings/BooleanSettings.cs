﻿using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Core.Fields.Validators;

namespace Krakenar.Core.Fields.Settings;

public record BooleanSettings : FieldTypeSettings, IBooleanSettings
{
  public override DataType DataType => DataType.Boolean;

  [JsonConstructor]
  public BooleanSettings()
  {
    new BooleanSettingsValidator().ValidateAndThrow(this);
  }

  public BooleanSettings(IBooleanSettings _) : this()
  {
  }
}
