import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-edit-inline',
  templateUrl: './edit-inline.component.html',
  styleUrls: ['./edit-inline.component.scss']
})
export class EditInlineComponent implements OnInit {

  @Input() data: any;
  @Output() focusOut: EventEmitter<any> = new EventEmitter<any>();
  currency = '$';
  editMode = false;
  constructor() {}

  ngOnInit() {}

  onFocusOut() {
    this.focusOut.emit(this.data);
  }

}
