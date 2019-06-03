import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MatDatepickerInputEvent } from '@angular/material';

@Component({
  selector: 'app-edit-inline',
  templateUrl: './edit-inline.component.html',
  styleUrls: ['./edit-inline.component.scss']
})
export class EditInlineComponent implements OnInit {

  @Input() data: any;
  @Input() type: string;
  @Output() focusOut: EventEmitter<any> = new EventEmitter<any>();

  @Output() dateChange: EventEmitter<MatDatepickerInputEvent<any>>;

  editMode = false;
  constructor() {}
  pickerAA = "asdasda"
  ngOnInit() {
    // console.log(this.data)
    // console.log(this.type)
  }

  onFocusOut() {
     console.log('onFocusOut', typeof this.data)
    this.focusOut.emit(this.data);
  }

  orgValueChange(event) {
    this.data = event.value
    this.focusOut.emit(this.data);
    this.editMode = false;
  }
}
