import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-participant-trash-modal',
  templateUrl: './participant-trash-modal.html',
  styleUrl: './participant-trash-modal.scss',
  host: { class: 'active' } 
})
export class ParticipantTrashModal {
  @Input() onConfirm?: () => void;
  @Input() onCancel?: () => void;

  confirm(): void {
    this.onConfirm?.();
  }

  cancel(): void {
    this.onCancel?.();
  }
}
