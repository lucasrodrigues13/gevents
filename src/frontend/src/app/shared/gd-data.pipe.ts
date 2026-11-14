import { Pipe, PipeTransform } from '@angular/core';
import { formatarDataBr } from './data';

@Pipe({ name: 'gdData' })
export class GdDataPipe implements PipeTransform {
  transform(valor: string | Date | null | undefined): string {
    return formatarDataBr(valor) || '—';
  }
}
