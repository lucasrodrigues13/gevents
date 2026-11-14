import { Component, Input, forwardRef } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

/** Campo de data pt-BR (`dd/MM/yyyy`) ligado a `Date | null` no formulário. */
@Component({
  selector: 'app-campo-data',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatDatepickerModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CampoDataComponent),
      multi: true,
    },
  ],
  template: `
    <mat-form-field appearance="outline">
      <mat-label>{{ rotulo }}</mat-label>
      <input
        matInput
        [matDatepicker]="picker"
        [formControl]="controle"
        placeholder="dd/mm/aaaa"
      />
      <mat-datepicker-toggle matIconSuffix [for]="picker" />
      <mat-datepicker #picker />
    </mat-form-field>
  `,
})
export class CampoDataComponent implements ControlValueAccessor {
  @Input() rotulo = 'Data';

  protected readonly controle = new FormControl<Date | null>(null);

  private propagar: (valor: Date | null) => void = () => undefined;
  private tocado: () => void = () => undefined;
  private sincronizando = false;

  constructor() {
    this.controle.valueChanges.subscribe((valor) => {
      if (this.sincronizando) {
        return;
      }
      this.propagar(valor);
      this.tocado();
    });
  }

  writeValue(valor: Date | null): void {
    this.sincronizando = true;
    this.controle.setValue(valor, { emitEvent: false });
    this.sincronizando = false;
  }

  registerOnChange(fn: (valor: Date | null) => void): void {
    this.propagar = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.tocado = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    if (isDisabled) {
      this.controle.disable({ emitEvent: false });
    } else {
      this.controle.enable({ emitEvent: false });
    }
  }
}
