import { Component, OnInit, Inject } from '@angular/core';
import {MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatTableDataSource } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';

@Component({
  selector: 'app-csv-config',
  templateUrl: './csv-config.component.html',
  styleUrls: ['./csv-config.component.css']
})
export class CsvConfigComponent {

  displayedColumns: string[] = ['isSave', 'displayProp', 'modelProp'];

  constructor(
    public dialogRef: MatDialogRef<CsvConfigComponent>,
    @Inject(MAT_DIALOG_DATA) public data: {itens: PropertyForSave[], resultOrder: string}) {}

  onNoClick(): void {
    this.dialogRef.close();
  }
}

export class PropertyForSave {
  constructor(displayProp: string, modelProp: string , isSave: boolean, orderBy: boolean) {
    this.displayProp = displayProp;
    this.modelProp = modelProp;
    this.isSave = isSave;
    this.orderBy = orderBy;
  }
    displayProp: string;
    modelProp: string;
    isSave: boolean;
    orderBy: boolean;
  }

