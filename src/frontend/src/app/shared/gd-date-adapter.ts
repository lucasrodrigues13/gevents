import { Injectable } from '@angular/core';
import { NativeDateAdapter } from '@angular/material/core';
import { formatarDataBr, parseDataBr } from './data';

@Injectable()
export class GdDateAdapter extends NativeDateAdapter {
  override parse(value: unknown): Date | null {
    if (typeof value === 'string') {
      return parseDataBr(value);
    }

    return super.parse(value as string);
  }

  override format(date: Date, displayFormat: Object): string {
    if (displayFormat === 'input' || this.ehEntrada(displayFormat)) {
      return formatarDataBr(date);
    }

    return super.format(date, displayFormat);
  }

  private ehEntrada(displayFormat: unknown): boolean {
    if (!displayFormat || typeof displayFormat !== 'object') {
      return false;
    }

    const fmt = displayFormat as Record<string, string>;
    return fmt['day'] === '2-digit' && fmt['month'] === '2-digit' && fmt['year'] === 'numeric';
  }
}

export const GD_DATE_FORMATS = {
  parse: {
    dateInput: 'input',
  },
  display: {
    dateInput: {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
    },
    monthYearLabel: {
      year: 'numeric',
      month: 'short',
    },
    dateA11yLabel: {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    },
    monthYearA11yLabel: {
      year: 'numeric',
      month: 'long',
    },
  },
};
