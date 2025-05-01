export type CustomAttribute = {
  key: string;
  value: string;
};

export type CustomIdentifier = {
  key: string;
  value: string;
};

export interface ICustomAttributeState {
  getInitialKey(): string;
  getInitialValue(): string;
  getCurrentKey(): string;
  getCurrentValue(): string;
  setCurrentKey(key: string): void;
  setCurrentValue(value: string): void;
  remove(): void;
  restore(): void;
  status: CustomAttributeStatus | undefined;
}

export class CustomAttributeState implements ICustomAttributeState {
  private initialKey: string = "";
  private initialValue: string = "";
  private currentKey: string;
  private currentValue: string;
  private isRemoved: boolean = false;

  constructor();
  constructor(key: string, value: string);
  constructor(attribute: CustomAttribute);
  constructor(arg1?: string | CustomAttribute, arg2?: string) {
    if (typeof arg1 === "object" && arg1 !== null) {
      this.initialKey = arg1.key.trim();
      this.initialValue = arg1.value.trim();
    } else if (typeof arg1 === "string" && typeof arg2 === "string") {
      this.initialKey = arg1;
      this.initialValue = arg2;
    }
    this.currentKey = this.initialKey;
    this.currentValue = this.initialValue;
  }

  get status(): CustomAttributeStatus | undefined {
    if (this.initialKey === "" && this.initialValue === "") {
      return "added";
    } else if (this.isRemoved) {
      return "removed";
    } else if (this.initialKey !== this.currentKey || this.initialValue !== this.currentValue) {
      return "updated";
    }
    return undefined;
  }

  getInitialKey(): string {
    return this.initialKey;
  }
  getInitialValue(): string {
    return this.initialValue;
  }

  getCurrentKey(): string {
    return this.currentKey;
  }
  getCurrentValue(): string {
    return this.currentValue;
  }

  setCurrentKey(key: string): void {
    this.currentKey = key;
  }
  setCurrentValue(value: string): void {
    this.currentValue = value;
  }

  remove(): void {
    this.isRemoved = true;
  }
  restore(): void {
    this.isRemoved = false;
  }
}

export type CustomAttributeStatus = "added" | "removed" | "updated";
