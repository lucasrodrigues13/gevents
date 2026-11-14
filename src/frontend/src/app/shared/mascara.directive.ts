import { Directive, ElementRef, HostListener, Input, inject } from '@angular/core';
import { NgControl } from '@angular/forms';
import { TipoMascara, aplicarMascara } from './mascaras';

@Directive({
  selector: 'input[gdMascara]',
})
export class MascaraDirective {
  @Input({ alias: 'gdMascara', required: true }) tipo!: TipoMascara;

  private readonly el = inject(ElementRef<HTMLInputElement>);
  private readonly controle = inject(NgControl, { optional: true, self: true });

  @HostListener('input')
  onInput(): void {
    this.aplicar(this.el.nativeElement.value);
  }

  @HostListener('blur')
  onBlur(): void {
    this.aplicar(this.el.nativeElement.value);
  }

  private aplicar(valor: string): void {
    const mascarado = aplicarMascara(valor, this.tipo);
    if (this.el.nativeElement.value !== mascarado) {
      this.el.nativeElement.value = mascarado;
    }

    const controle = this.controle?.control;
    if (controle && controle.value !== mascarado) {
      controle.setValue(mascarado, { emitEvent: false });
    }
  }
}
