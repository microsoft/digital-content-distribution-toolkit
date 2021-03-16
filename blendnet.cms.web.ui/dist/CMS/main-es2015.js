(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["main"],{

/***/ 0:
/*!***************************!*\
  !*** multi ./src/main.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(/*! C:\BlendNet\bine-crm\blendnet.cms.web.ui\src\main.ts */"zUnb");


/***/ }),

/***/ "AytR":
/*!*****************************************!*\
  !*** ./src/environments/environment.ts ***!
  \*****************************************/
/*! exports provided: environment */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "environment", function() { return environment; });
const environment = {
    production: false,
    oidcSettings: {
        client_id: "8c495645-d008-4371-9128-718b766d7f76",
        authority: "https://mishtudev.b2clogin.com/tfp/mishtudev/b2c_1a_signuporsigninwithphone",
        response_type: "id_token token",
        post_logout_redirect_uri: "http://localhost:4200/",
        loadUserInfo: false,
        redirect_uri: "http://localhost:4200/",
        silent_redirect_uri: "http://localhost:4200/",
        response_mode: "fragment",
        scope: 'https://mishtudev.onmicrosoft.com/68fb9dd5-8d1b-4d00-9eaf-d781db510c7f/user.impersonation'
    }
};


/***/ }),

/***/ "DGPo":
/*!******************************************************!*\
  !*** ./src/app/broadcasted/broadcasted.component.ts ***!
  \******************************************************/
/*! exports provided: BroadcastedComponent, BroadcastConfirmDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BroadcastedComponent", function() { return BroadcastedComponent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BroadcastConfirmDialog", function() { return BroadcastConfirmDialog; });
/* harmony import */ var _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/cdk/collections */ "0EQZ");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material/dialog */ "0IaG");
/* harmony import */ var _angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/paginator */ "M9IT");
/* harmony import */ var _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/sort */ "Dh3D");
/* harmony import */ var _angular_material_table__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/table */ "+0xr");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_material_form_field__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/form-field */ "kmnG");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/input */ "qFsG");
/* harmony import */ var _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/checkbox */ "bSwM");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/button */ "bTqV");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/icon */ "NFeN");
/* harmony import */ var _angular_material_list__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/list */ "MutI");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/tooltip */ "Qu3c");

















function BroadcastedComponent_th_12_Template(rf, ctx) { if (rf & 1) {
    const _r17 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "mat-checkbox", 20);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("change", function BroadcastedComponent_th_12_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r17); const ctx_r16 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return $event ? ctx_r16.masterToggle() : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r1 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("checked", ctx_r1.selection.hasValue() && ctx_r1.isAllSelected())("indeterminate", ctx_r1.selection.hasValue() && !ctx_r1.isAllSelected());
} }
function BroadcastedComponent_td_13_Template(rf, ctx) { if (rf & 1) {
    const _r21 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "mat-checkbox", 22);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function BroadcastedComponent_td_13_Template_mat_checkbox_click_1_listener($event) { return $event.stopPropagation(); })("change", function BroadcastedComponent_td_13_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r21); const row_r18 = ctx.$implicit; const ctx_r20 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return $event ? ctx_r20.selection.toggle(row_r18) : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r18 = ctx.$implicit;
    const ctx_r2 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("checked", ctx_r2.selection.isSelected(row_r18));
} }
function BroadcastedComponent_th_15_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " ID ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function BroadcastedComponent_td_16_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r22 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r22.id, " ");
} }
function BroadcastedComponent_th_18_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " Status ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function BroadcastedComponent_td_19_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r23 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r23.status, " ");
} }
function BroadcastedComponent_th_21_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " Content Name ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function BroadcastedComponent_td_22_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r24 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r24.name, " ");
} }
function BroadcastedComponent_th_24_Template(rf, ctx) { if (rf & 1) {
    const _r26 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function BroadcastedComponent_th_24_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r26); const ctx_r25 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r25.openStopBroadcastConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "cancel");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function BroadcastedComponent_td_25_Template(rf, ctx) { if (rf & 1) {
    const _r29 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 26);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function BroadcastedComponent_td_25_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r29); const ctx_r28 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r28.openStopBroadcastConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 27);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "cancel");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r27 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("disabled", !row_r27.isBroadcastCancellable);
} }
function BroadcastedComponent_th_27_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 28);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2, " Manage Devices ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function BroadcastedComponent_td_28_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 29);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2, " Manage Devices ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function BroadcastedComponent_tr_29_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](0, "tr", 30);
} }
function BroadcastedComponent_tr_30_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](0, "tr", 31);
} }
function BroadcastedComponent_tr_31_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "tr", 32);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "td", 33);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    const _r0 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵreference"](7);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"]("No data matching the filter \"", _r0.value, "\"");
} }
const _c0 = function () { return [5, 10, 25, 100]; };
const NAMES = [
    'Dabangg', 'Bajrangi Bhaijaan', 'Don', 'RamLeela', 'Race 3', 'KingKong'
];
/**
 * @title Data table with sorting, pagination, and filtering.
 */
class BroadcastedComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this.displayedColumns = ['select', 'id', 'name', 'status', 'isBroadcastCancellable', 'manageDevice'];
        this.fileToUpload = null;
        this.showDialog = false;
        this.message = "Please press OK to continue.";
        this.initialSelection = [];
        this.allowMultiSelect = true;
        this.selection = new _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_0__["SelectionModel"](this.allowMultiSelect, this.initialSelection);
        // Create 100 users
        const users = Array.from({ length: 100 }, (_, k) => createNewUser(k + 1));
        // Assign the data to the data source for the table to render
        this.dataSource = new _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatTableDataSource"](users);
    }
    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }
    applyFilter(event) {
        const filterValue = event.target.value;
        this.dataSource.filter = filterValue.trim().toLowerCase();
        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
    }
    isAllSelected() {
        const numSelected = this.selection.selected.length;
        const numRows = this.dataSource.data.length;
        return numSelected == numRows;
    }
    masterToggle() {
        this.isAllSelected() ?
            this.selection.clear() :
            this.dataSource.data.forEach(row => this.selection.select(row));
    }
    openStopBroadcastConfirmModal() {
        const dialogRef = this.dialog.open(BroadcastConfirmDialog, {
            data: { message: this.message },
            width: '40%'
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
}
BroadcastedComponent.ɵfac = function BroadcastedComponent_Factory(t) { return new (t || BroadcastedComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MatDialog"])); };
BroadcastedComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdefineComponent"]({ type: BroadcastedComponent, selectors: [["app-broadcasted"]], viewQuery: function BroadcastedComponent_Query(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵviewQuery"](_angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__["MatPaginator"], 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵviewQuery"](_angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSort"], 1);
    } if (rf & 2) {
        let _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵloadQuery"]()) && (ctx.paginator = _t.first);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵloadQuery"]()) && (ctx.sort = _t.first);
    } }, decls: 33, vars: 5, consts: [[1, "cms-container"], ["matInput", "", "placeholder", "Ex. Don", 3, "keyup"], ["input", ""], [1, "mat-elevation-z8"], [1, "cms-table-container"], ["mat-table", "", "matSort", "", 3, "dataSource"], ["matColumnDef", "select"], ["mat-header-cell", "", 4, "matHeaderCellDef"], ["mat-cell", "", 4, "matCellDef"], ["matColumnDef", "id"], ["mat-header-cell", "", "mat-sort-header", "", 4, "matHeaderCellDef"], ["matColumnDef", "status"], ["matColumnDef", "name"], ["matColumnDef", "isBroadcastCancellable"], ["matColumnDef", "manageDevice"], ["mat-header-row", "", 4, "matHeaderRowDef"], ["mat-row", "", 4, "matRowDef", "matRowDefColumns"], ["class", "mat-row", 4, "matNoDataRow"], [3, "pageSizeOptions"], ["mat-header-cell", ""], [3, "checked", "indeterminate", "change"], ["mat-cell", ""], [3, "checked", "click", "change"], ["mat-header-cell", "", "mat-sort-header", ""], ["mat-icon-button", "", 3, "click"], ["mat-list-icon", ""], ["mat-icon-button", "", 3, "disabled", "click"], ["mat-list-icon", "", "matTooltip", "Cancel Broadcast"], ["mat-button", "", "color", "primary", "disabled", ""], ["mat-button", "", "color", "primary"], ["mat-header-row", ""], ["mat-row", ""], [1, "mat-row"], ["colspan", "4", 1, "mat-cell"]], template: function BroadcastedComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "h1");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2, "Broadcasted Content");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "mat-form-field");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](4, "mat-label");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](5, "Filter");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](6, "input", 1, 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("keyup", function BroadcastedComponent_Template_input_keyup_6_listener($event) { return ctx.applyFilter($event); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](8, "div", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](9, "div", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](10, "table", 5);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](11, 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](12, BroadcastedComponent_th_12_Template, 2, 2, "th", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](13, BroadcastedComponent_td_13_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](14, 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](15, BroadcastedComponent_th_15_Template, 2, 0, "th", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](16, BroadcastedComponent_td_16_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](17, 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](18, BroadcastedComponent_th_18_Template, 2, 0, "th", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](19, BroadcastedComponent_td_19_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](20, 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](21, BroadcastedComponent_th_21_Template, 2, 0, "th", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](22, BroadcastedComponent_td_22_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](23, 13);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](24, BroadcastedComponent_th_24_Template, 4, 0, "th", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](25, BroadcastedComponent_td_25_Template, 4, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](26, 14);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](27, BroadcastedComponent_th_27_Template, 3, 0, "th", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](28, BroadcastedComponent_td_28_Template, 3, 0, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](29, BroadcastedComponent_tr_29_Template, 1, 0, "tr", 15);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](30, BroadcastedComponent_tr_30_Template, 1, 0, "tr", 16);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](31, BroadcastedComponent_tr_31_Template, 3, 1, "tr", 17);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](32, "mat-paginator", 18);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("dataSource", ctx.dataSource);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](19);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("matHeaderRowDef", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("matRowDefColumns", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("pageSizeOptions", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](4, _c0));
    } }, directives: [_angular_material_form_field__WEBPACK_IMPORTED_MODULE_6__["MatFormField"], _angular_material_form_field__WEBPACK_IMPORTED_MODULE_6__["MatLabel"], _angular_material_input__WEBPACK_IMPORTED_MODULE_7__["MatInput"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatTable"], _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSort"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatColumnDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderCellDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatCellDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderRowDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatRowDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatNoDataRow"], _angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__["MatPaginator"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderCell"], _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_8__["MatCheckbox"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatCell"], _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSortHeader"], _angular_material_button__WEBPACK_IMPORTED_MODULE_9__["MatButton"], _angular_material_icon__WEBPACK_IMPORTED_MODULE_10__["MatIcon"], _angular_material_list__WEBPACK_IMPORTED_MODULE_11__["MatListIconCssMatStyler"], _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_12__["MatTooltip"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderRow"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatRow"]], styles: ["table[_ngcontent-%COMP%] {\r\n    width: 100%;\r\n  }\r\n  \r\n  .mat-form-field[_ngcontent-%COMP%] {\r\n    font-size: 14px;\r\n    width: 100%;\r\n  }\r\n  \r\n  th.mat-header-cell[_ngcontent-%COMP%]:last-of-type, td.mat-cell[_ngcontent-%COMP%]:last-of-type, td.mat-footer-cell[_ngcontent-%COMP%]:last-of-type {\r\n\tpadding-right: 0px !important\r\n}\r\n  \r\n  .cms-container[_ngcontent-%COMP%] {\r\n    height: 80%;\r\n    width: 80%;\r\n    margin-left: 10%;\r\n    margin-top: 5%;\r\n  }\r\n  \r\n  .cms-table-container[_ngcontent-%COMP%] {\r\n    position: relative;\r\n    max-height: 400px;\r\n    overflow: auto;\r\n  }\r\n  \r\n  .btn-sec[_ngcontent-%COMP%] {\r\n    margin-right: 3rem !important;\r\n    text-align: right;\r\n    margin-top: 1.5em;\r\n  }\r\n  \r\n  .update-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\r\n  \r\n  .discard-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbImJyb2FkY2FzdGVkLmNvbXBvbmVudC5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSxXQUFXO0VBQ2I7O0VBRUE7SUFDRSxlQUFlO0lBQ2YsV0FBVztFQUNiOztFQUVGOzs7Q0FHQztBQUNEOztFQUdFO0lBQ0UsV0FBVztJQUNYLFVBQVU7SUFDVixnQkFBZ0I7SUFDaEIsY0FBYztFQUNoQjs7RUFFQTtJQUNFLGtCQUFrQjtJQUNsQixpQkFBaUI7SUFDakIsY0FBYztFQUNoQjs7RUFHQTtJQUNFLDZCQUE2QjtJQUM3QixpQkFBaUI7SUFDakIsaUJBQWlCO0VBQ25COztFQUNBO0lBQ0UsV0FBVztFQUNiOztFQUNBO0lBQ0UsV0FBVztFQUNiIiwiZmlsZSI6ImJyb2FkY2FzdGVkLmNvbXBvbmVudC5jc3MiLCJzb3VyY2VzQ29udGVudCI6WyJ0YWJsZSB7XHJcbiAgICB3aWR0aDogMTAwJTtcclxuICB9XHJcbiAgXHJcbiAgLm1hdC1mb3JtLWZpZWxkIHtcclxuICAgIGZvbnQtc2l6ZTogMTRweDtcclxuICAgIHdpZHRoOiAxMDAlO1xyXG4gIH1cclxuICBcclxudGgubWF0LWhlYWRlci1jZWxsOmxhc3Qtb2YtdHlwZSxcclxudGQubWF0LWNlbGw6bGFzdC1vZi10eXBlLFxyXG50ZC5tYXQtZm9vdGVyLWNlbGw6bGFzdC1vZi10eXBlIHtcclxuXHRwYWRkaW5nLXJpZ2h0OiAwcHggIWltcG9ydGFudFxyXG59XHJcblxyXG4gIFxyXG4gIC5jbXMtY29udGFpbmVyIHtcclxuICAgIGhlaWdodDogODAlO1xyXG4gICAgd2lkdGg6IDgwJTtcclxuICAgIG1hcmdpbi1sZWZ0OiAxMCU7XHJcbiAgICBtYXJnaW4tdG9wOiA1JTtcclxuICB9XHJcblxyXG4gIC5jbXMtdGFibGUtY29udGFpbmVyIHtcclxuICAgIHBvc2l0aW9uOiByZWxhdGl2ZTtcclxuICAgIG1heC1oZWlnaHQ6IDQwMHB4O1xyXG4gICAgb3ZlcmZsb3c6IGF1dG87XHJcbiAgfVxyXG4gIFxyXG5cclxuICAuYnRuLXNlYyB7XHJcbiAgICBtYXJnaW4tcmlnaHQ6IDNyZW0gIWltcG9ydGFudDtcclxuICAgIHRleHQtYWxpZ246IHJpZ2h0O1xyXG4gICAgbWFyZ2luLXRvcDogMS41ZW07XHJcbiAgfVxyXG4gIC51cGRhdGUtYnRuIHtcclxuICAgIG1hcmdpbjogNXB4O1xyXG4gIH1cclxuICAuZGlzY2FyZC1idG4ge1xyXG4gICAgbWFyZ2luOiA1cHg7XHJcbiAgfSJdfQ== */"] });
class BroadcastConfirmDialog {
    constructor(dialogRef, data) {
        this.dialogRef = dialogRef;
        this.data = data;
    }
    onCancelUpload() {
        this.dialogRef.close();
    }
    onConfirmUpload() {
        this.dialogRef.close();
    }
}
BroadcastConfirmDialog.ɵfac = function BroadcastConfirmDialog_Factory(t) { return new (t || BroadcastConfirmDialog)(_angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MatDialogRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MAT_DIALOG_DATA"])); };
BroadcastConfirmDialog.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdefineComponent"]({ type: BroadcastConfirmDialog, selectors: [["broadcasted-confirm-dialog"]], decls: 10, vars: 1, consts: [["mat-dialog-title", ""], ["mat-dialog-content", ""], ["mat-dialog-actions", "", 1, "btn-sec"], ["mat-raised-button", "", "mat-dialog-close", "", 1, "discard-btn", 3, "click"], ["mat-raised-button", "", "color", "primary", 1, "update-btn", 3, "click"]], template: function BroadcastConfirmDialog_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "h2", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, "Confirm");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "div", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "p");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](5, "div", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](6, "button", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function BroadcastConfirmDialog_Template_button_click_6_listener() { return ctx.onCancelUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](7, "Cancel");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](8, "button", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function BroadcastConfirmDialog_Template_button_click_8_listener() { return ctx.onConfirmUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](9, "OK");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate"](ctx.data.message);
    } }, encapsulation: 2 });
/** Builds and returns a new User. */
function createNewUser(id) {
    const name = NAMES[Math.round(Math.random() * (NAMES.length - 1))];
    const status = id % 2 === 0 ? 'Active Broadcast' : 'Cancelling Broadcast';
    const isBroadcastCancellableVal = status === 'Active Broadcast' ? true : false;
    return {
        id: id.toString(),
        name: name,
        status: status,
        isBroadcastCancellable: isBroadcastCancellableVal,
    };
}


/***/ }),

/***/ "Oj9r":
/*!**************************************************!*\
  !*** ./src/app/processed/processed.component.ts ***!
  \**************************************************/
/*! exports provided: ProcessedComponent, ProcessConfirmDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProcessedComponent", function() { return ProcessedComponent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProcessConfirmDialog", function() { return ProcessConfirmDialog; });
/* harmony import */ var _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/cdk/collections */ "0EQZ");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material/dialog */ "0IaG");
/* harmony import */ var _angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/paginator */ "M9IT");
/* harmony import */ var _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/sort */ "Dh3D");
/* harmony import */ var _angular_material_table__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/table */ "+0xr");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_material_form_field__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/form-field */ "kmnG");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/input */ "qFsG");
/* harmony import */ var _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/checkbox */ "bSwM");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/button */ "bTqV");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/icon */ "NFeN");
/* harmony import */ var _angular_material_list__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/list */ "MutI");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/tooltip */ "Qu3c");

















function ProcessedComponent_th_12_Template(rf, ctx) { if (rf & 1) {
    const _r17 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "mat-checkbox", 20);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("change", function ProcessedComponent_th_12_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r17); const ctx_r16 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return $event ? ctx_r16.masterToggle() : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r1 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("checked", ctx_r1.selection.hasValue() && ctx_r1.isAllSelected())("indeterminate", ctx_r1.selection.hasValue() && !ctx_r1.isAllSelected());
} }
function ProcessedComponent_td_13_Template(rf, ctx) { if (rf & 1) {
    const _r21 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "mat-checkbox", 22);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function ProcessedComponent_td_13_Template_mat_checkbox_click_1_listener($event) { return $event.stopPropagation(); })("change", function ProcessedComponent_td_13_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r21); const row_r18 = ctx.$implicit; const ctx_r20 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return $event ? ctx_r20.selection.toggle(row_r18) : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r18 = ctx.$implicit;
    const ctx_r2 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("checked", ctx_r2.selection.isSelected(row_r18));
} }
function ProcessedComponent_th_15_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " ID ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function ProcessedComponent_td_16_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r22 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r22.id, " ");
} }
function ProcessedComponent_th_18_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " Status ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function ProcessedComponent_td_19_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r23 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r23.status, " ");
} }
function ProcessedComponent_th_21_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " Content Name ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function ProcessedComponent_td_22_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r24 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r24.name, " ");
} }
function ProcessedComponent_th_24_Template(rf, ctx) { if (rf & 1) {
    const _r26 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function ProcessedComponent_th_24_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r26); const ctx_r25 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r25.openBroadcastConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "podcasts");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function ProcessedComponent_td_25_Template(rf, ctx) { if (rf & 1) {
    const _r29 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 26);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function ProcessedComponent_td_25_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r29); const ctx_r28 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r28.openBroadcastConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 27);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "podcasts");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r27 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("disabled", !row_r27.isBroadcastable);
} }
function ProcessedComponent_th_27_Template(rf, ctx) { if (rf & 1) {
    const _r31 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function ProcessedComponent_th_27_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r31); const ctx_r30 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r30.openDeleteConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "delete");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function ProcessedComponent_td_28_Template(rf, ctx) { if (rf & 1) {
    const _r34 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 26);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function ProcessedComponent_td_28_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r34); const ctx_r33 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r33.openDeleteConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 28);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "delete");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r32 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("disabled", !row_r32.isDeletable);
} }
function ProcessedComponent_tr_29_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](0, "tr", 29);
} }
function ProcessedComponent_tr_30_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](0, "tr", 30);
} }
function ProcessedComponent_tr_31_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "tr", 31);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "td", 32);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    const _r0 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵreference"](7);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"]("No data matching the filter \"", _r0.value, "\"");
} }
const _c0 = function () { return [5, 10, 25, 100]; };
const NAMES = [
    'Dabangg', 'Bajrangi Bhaijaan', 'Don', 'RamLeela', 'Race 3', 'KingKong'
];
/**
 * @title Data table with sorting, pagination, and filtering.
 */
class ProcessedComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this.displayedColumns = ['select', 'id', 'name', 'status', 'isBroadcastable', 'isDeletable'];
        this.fileToUpload = null;
        this.showDialog = false;
        this.message = "Please press OK to continue.";
        this.initialSelection = [];
        this.allowMultiSelect = true;
        this.selection = new _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_0__["SelectionModel"](this.allowMultiSelect, this.initialSelection);
        // Create 100 users
        const users = Array.from({ length: 100 }, (_, k) => createNewUser(k + 1));
        // Assign the data to the data source for the table to render
        this.dataSource = new _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatTableDataSource"](users);
    }
    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }
    applyFilter(event) {
        const filterValue = event.target.value;
        this.dataSource.filter = filterValue.trim().toLowerCase();
        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
    }
    isAllSelected() {
        const numSelected = this.selection.selected.length;
        const numRows = this.dataSource.data.length;
        return numSelected == numRows;
    }
    masterToggle() {
        this.isAllSelected() ?
            this.selection.clear() :
            this.dataSource.data.forEach(row => this.selection.select(row));
    }
    openDeleteConfirmModal() {
        const dialogRef = this.dialog.open(ProcessConfirmDialog, {
            data: { message: this.message },
            width: '40%'
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
    openBroadcastConfirmModal() {
        const dialogRef = this.dialog.open(ProcessConfirmDialog, {
            data: { message: this.message },
            width: '40%'
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
}
ProcessedComponent.ɵfac = function ProcessedComponent_Factory(t) { return new (t || ProcessedComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MatDialog"])); };
ProcessedComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdefineComponent"]({ type: ProcessedComponent, selectors: [["app-processed"]], viewQuery: function ProcessedComponent_Query(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵviewQuery"](_angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__["MatPaginator"], 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵviewQuery"](_angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSort"], 1);
    } if (rf & 2) {
        let _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵloadQuery"]()) && (ctx.paginator = _t.first);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵloadQuery"]()) && (ctx.sort = _t.first);
    } }, decls: 33, vars: 5, consts: [[1, "cms-container"], ["matInput", "", "placeholder", "Ex. Don", 3, "keyup"], ["input", ""], [1, "mat-elevation-z8"], [1, "cms-table-container"], ["mat-table", "", "matSort", "", 3, "dataSource"], ["matColumnDef", "select"], ["mat-header-cell", "", 4, "matHeaderCellDef"], ["mat-cell", "", 4, "matCellDef"], ["matColumnDef", "id"], ["mat-header-cell", "", "mat-sort-header", "", 4, "matHeaderCellDef"], ["matColumnDef", "status"], ["matColumnDef", "name"], ["matColumnDef", "isBroadcastable"], ["matColumnDef", "isDeletable"], ["mat-header-row", "", 4, "matHeaderRowDef"], ["mat-row", "", 4, "matRowDef", "matRowDefColumns"], ["class", "mat-row", 4, "matNoDataRow"], [3, "pageSizeOptions"], ["mat-header-cell", ""], [3, "checked", "indeterminate", "change"], ["mat-cell", ""], [3, "checked", "click", "change"], ["mat-header-cell", "", "mat-sort-header", ""], ["mat-icon-button", "", 3, "click"], ["mat-list-icon", ""], ["mat-icon-button", "", 3, "disabled", "click"], ["mat-list-icon", "", "matTooltip", "Start broadcast"], ["mat-list-icon", "", "matTooltip", "Archive Content"], ["mat-header-row", ""], ["mat-row", ""], [1, "mat-row"], ["colspan", "4", 1, "mat-cell"]], template: function ProcessedComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "h1");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2, "Processed Content");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "mat-form-field");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](4, "mat-label");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](5, "Filter");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](6, "input", 1, 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("keyup", function ProcessedComponent_Template_input_keyup_6_listener($event) { return ctx.applyFilter($event); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](8, "div", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](9, "div", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](10, "table", 5);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](11, 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](12, ProcessedComponent_th_12_Template, 2, 2, "th", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](13, ProcessedComponent_td_13_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](14, 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](15, ProcessedComponent_th_15_Template, 2, 0, "th", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](16, ProcessedComponent_td_16_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](17, 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](18, ProcessedComponent_th_18_Template, 2, 0, "th", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](19, ProcessedComponent_td_19_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](20, 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](21, ProcessedComponent_th_21_Template, 2, 0, "th", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](22, ProcessedComponent_td_22_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](23, 13);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](24, ProcessedComponent_th_24_Template, 4, 0, "th", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](25, ProcessedComponent_td_25_Template, 4, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](26, 14);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](27, ProcessedComponent_th_27_Template, 4, 0, "th", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](28, ProcessedComponent_td_28_Template, 4, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](29, ProcessedComponent_tr_29_Template, 1, 0, "tr", 15);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](30, ProcessedComponent_tr_30_Template, 1, 0, "tr", 16);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](31, ProcessedComponent_tr_31_Template, 3, 1, "tr", 17);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](32, "mat-paginator", 18);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("dataSource", ctx.dataSource);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](19);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("matHeaderRowDef", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("matRowDefColumns", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("pageSizeOptions", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](4, _c0));
    } }, directives: [_angular_material_form_field__WEBPACK_IMPORTED_MODULE_6__["MatFormField"], _angular_material_form_field__WEBPACK_IMPORTED_MODULE_6__["MatLabel"], _angular_material_input__WEBPACK_IMPORTED_MODULE_7__["MatInput"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatTable"], _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSort"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatColumnDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderCellDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatCellDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderRowDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatRowDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatNoDataRow"], _angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__["MatPaginator"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderCell"], _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_8__["MatCheckbox"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatCell"], _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSortHeader"], _angular_material_button__WEBPACK_IMPORTED_MODULE_9__["MatButton"], _angular_material_icon__WEBPACK_IMPORTED_MODULE_10__["MatIcon"], _angular_material_list__WEBPACK_IMPORTED_MODULE_11__["MatListIconCssMatStyler"], _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_12__["MatTooltip"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderRow"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatRow"]], styles: ["table[_ngcontent-%COMP%] {\r\n    width: 100%;\r\n  }\r\n  \r\n  .mat-form-field[_ngcontent-%COMP%] {\r\n    font-size: 14px;\r\n    width: 100%;\r\n  }\r\n  \r\n  th.mat-header-cell[_ngcontent-%COMP%]:last-of-type, td.mat-cell[_ngcontent-%COMP%]:last-of-type, td.mat-footer-cell[_ngcontent-%COMP%]:last-of-type {\r\n\tpadding-right: 0px !important\r\n}\r\n  \r\n  .cms-container[_ngcontent-%COMP%] {\r\n    height: 80%;\r\n    width: 80%;\r\n    margin-left: 10%;\r\n    margin-top: 5%;\r\n  }\r\n  \r\n  .cms-table-container[_ngcontent-%COMP%] {\r\n    position: relative;\r\n    max-height: 400px;\r\n    overflow: auto;\r\n  }\r\n  \r\n  .btn-sec[_ngcontent-%COMP%] {\r\n    margin-right: 3rem !important;\r\n    text-align: right;\r\n    margin-top: 1.5em;\r\n  }\r\n  \r\n  .update-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\r\n  \r\n  .discard-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2Nlc3NlZC5jb21wb25lbnQuY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0lBQ0ksV0FBVztFQUNiOztFQUVBO0lBQ0UsZUFBZTtJQUNmLFdBQVc7RUFDYjs7RUFFRjs7O0NBR0M7QUFDRDs7RUFHRTtJQUNFLFdBQVc7SUFDWCxVQUFVO0lBQ1YsZ0JBQWdCO0lBQ2hCLGNBQWM7RUFDaEI7O0VBRUE7SUFDRSxrQkFBa0I7SUFDbEIsaUJBQWlCO0lBQ2pCLGNBQWM7RUFDaEI7O0VBQ0E7SUFDRSw2QkFBNkI7SUFDN0IsaUJBQWlCO0lBQ2pCLGlCQUFpQjtFQUNuQjs7RUFDQTtJQUNFLFdBQVc7RUFDYjs7RUFDQTtJQUNFLFdBQVc7RUFDYiIsImZpbGUiOiJwcm9jZXNzZWQuY29tcG9uZW50LmNzcyIsInNvdXJjZXNDb250ZW50IjpbInRhYmxlIHtcclxuICAgIHdpZHRoOiAxMDAlO1xyXG4gIH1cclxuICBcclxuICAubWF0LWZvcm0tZmllbGQge1xyXG4gICAgZm9udC1zaXplOiAxNHB4O1xyXG4gICAgd2lkdGg6IDEwMCU7XHJcbiAgfVxyXG4gIFxyXG50aC5tYXQtaGVhZGVyLWNlbGw6bGFzdC1vZi10eXBlLFxyXG50ZC5tYXQtY2VsbDpsYXN0LW9mLXR5cGUsXHJcbnRkLm1hdC1mb290ZXItY2VsbDpsYXN0LW9mLXR5cGUge1xyXG5cdHBhZGRpbmctcmlnaHQ6IDBweCAhaW1wb3J0YW50XHJcbn1cclxuXHJcbiAgXHJcbiAgLmNtcy1jb250YWluZXIge1xyXG4gICAgaGVpZ2h0OiA4MCU7XHJcbiAgICB3aWR0aDogODAlO1xyXG4gICAgbWFyZ2luLWxlZnQ6IDEwJTtcclxuICAgIG1hcmdpbi10b3A6IDUlO1xyXG4gIH1cclxuXHJcbiAgLmNtcy10YWJsZS1jb250YWluZXIge1xyXG4gICAgcG9zaXRpb246IHJlbGF0aXZlO1xyXG4gICAgbWF4LWhlaWdodDogNDAwcHg7XHJcbiAgICBvdmVyZmxvdzogYXV0bztcclxuICB9XHJcbiAgLmJ0bi1zZWMge1xyXG4gICAgbWFyZ2luLXJpZ2h0OiAzcmVtICFpbXBvcnRhbnQ7XHJcbiAgICB0ZXh0LWFsaWduOiByaWdodDtcclxuICAgIG1hcmdpbi10b3A6IDEuNWVtO1xyXG4gIH1cclxuICAudXBkYXRlLWJ0biB7XHJcbiAgICBtYXJnaW46IDVweDtcclxuICB9XHJcbiAgLmRpc2NhcmQtYnRuIHtcclxuICAgIG1hcmdpbjogNXB4O1xyXG4gIH0iXX0= */"] });
class ProcessConfirmDialog {
    constructor(dialogRef, data) {
        this.dialogRef = dialogRef;
        this.data = data;
    }
    onCancelUpload() {
        this.dialogRef.close();
    }
    onConfirmUpload() {
        this.dialogRef.close();
    }
}
ProcessConfirmDialog.ɵfac = function ProcessConfirmDialog_Factory(t) { return new (t || ProcessConfirmDialog)(_angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MatDialogRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MAT_DIALOG_DATA"])); };
ProcessConfirmDialog.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdefineComponent"]({ type: ProcessConfirmDialog, selectors: [["process-confirm-dialog"]], decls: 10, vars: 1, consts: [["mat-dialog-title", ""], ["mat-dialog-content", ""], ["mat-dialog-actions", "", 1, "btn-sec"], ["mat-raised-button", "", "mat-dialog-close", "", 1, "discard-btn", 3, "click"], ["mat-raised-button", "", "color", "primary", 1, "update-btn", 3, "click"]], template: function ProcessConfirmDialog_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "h2", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, "Confirm");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "div", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "p");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](5, "div", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](6, "button", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function ProcessConfirmDialog_Template_button_click_6_listener() { return ctx.onCancelUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](7, "Cancel");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](8, "button", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function ProcessConfirmDialog_Template_button_click_8_listener() { return ctx.onConfirmUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](9, "OK");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate"](ctx.data.message);
    } }, encapsulation: 2 });
/** Builds and returns a new User. */
function createNewUser(id) {
    const name = NAMES[Math.round(Math.random() * (NAMES.length - 1))];
    const status = id % 2 === 0 ? 'Processed' : 'Broadcasting';
    const isBroadcastableVal = status === 'Processed' ? true : false;
    const isDeletableVal = status === 'Processed' ? true : false;
    return {
        id: id.toString(),
        name: name,
        status: status,
        isBroadcastable: isBroadcastableVal,
        isDeletable: isDeletableVal
    };
}


/***/ }),

/***/ "QTNb":
/*!************************************************************************!*\
  !*** ./src/app/add-content-provider/add-content-provider.component.ts ***!
  \************************************************************************/
/*! exports provided: AddContentProviderComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AddContentProviderComponent", function() { return AddContentProviderComponent; });
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/forms */ "3Pt+");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "0IaG");
/* harmony import */ var _angular_material_grid_list__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/grid-list */ "zkoq");
/* harmony import */ var _angular_material_form_field__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/form-field */ "kmnG");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/input */ "qFsG");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/common */ "ofXK");
/* harmony import */ var _angular_material_radio__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/radio */ "QibW");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/icon */ "NFeN");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/button */ "bTqV");










function AddContentProviderComponent_mat_error_10_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "mat-error");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1, " Please enter name ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function AddContentProviderComponent_mat_error_15_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "mat-error");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1, " Please enter logourl id ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
class AddContentProviderComponent {
    constructor(dialogRef) {
        this.dialogRef = dialogRef;
        this.cpname = new _angular_forms__WEBPACK_IMPORTED_MODULE_0__["FormControl"](' ', [_angular_forms__WEBPACK_IMPORTED_MODULE_0__["Validators"].required]);
        this.logourl = new _angular_forms__WEBPACK_IMPORTED_MODULE_0__["FormControl"](' ', [_angular_forms__WEBPACK_IMPORTED_MODULE_0__["Validators"].required]);
    }
    ngOnInit() {
    }
    closeDialog() {
        this.dialogRef.close();
    }
}
AddContentProviderComponent.ɵfac = function AddContentProviderComponent_Factory(t) { return new (t || AddContentProviderComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"])); };
AddContentProviderComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineComponent"]({ type: AddContentProviderComponent, selectors: [["app-add-content-provider"]], decls: 37, vars: 5, consts: [[1, "add-title"], [1, "primary"], ["cols", "2", "rowHeight", "85px"], [1, "text-inside"], ["matInput", "", "placeholder", "Name", "required", "", 3, "value"], [4, "ngIf"], [1, "example-full-width"], ["matInput", "", "placeholder", "Logo URL address", "required", "", 3, "value"], [1, "status"], [2, "color", "grey"], ["color", "primary", "value", "1", 2, "padding", "4px 4px 0px 0px"], ["color", "primary", "value", "2", 2, "padding", "4px 0px 0px 24px"], ["matInput", "", "placeholder", "Admin", 3, "value"], [1, "btn-sec"], ["mat-raised-button", "", "type", "submit", "color", "primary", 1, "update-btn"], ["mat-raised-button", "", "type", "button", 1, "discard-btn", 3, "click"]], template: function AddContentProviderComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "h2", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](3, "Add Content Provider");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](4, "div");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](5, "mat-grid-list", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](6, "mat-grid-tile");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](7, "div", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](8, "mat-form-field");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](9, "input", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](10, AddContentProviderComponent_mat_error_10_Template, 2, 0, "mat-error", 5);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](11, "mat-grid-tile");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](12, "div", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](13, "mat-form-field", 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](14, "input", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](15, AddContentProviderComponent_mat_error_15_Template, 2, 0, "mat-error", 5);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](16, "mat-grid-tile");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](17, "div", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](18, "mat-radio-group", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](19, "div", 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](20, "Status");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](21, "mat-radio-button", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](22, "Active");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](23, "mat-radio-button", 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](24, "Inactive");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](25, "mat-grid-tile");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](26, "mat-grid-tile");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](27, "div", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](28, "mat-form-field", 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](29, "input", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](30, "mat-icon");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](31, "search");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](32, "div", 13);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](33, "button", 14);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](34, "Save");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](35, "button", 15);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function AddContentProviderComponent_Template_button_click_35_listener() { return ctx.closeDialog(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](36, "Cancel");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](9);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵpropertyInterpolate"]("value", ctx.cpname);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx.cpname.invalid);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵpropertyInterpolate"]("value", ctx.logourl);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx.logourl.invalid);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](14);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵpropertyInterpolate"]("value", ctx.cpadmin);
    } }, directives: [_angular_material_grid_list__WEBPACK_IMPORTED_MODULE_3__["MatGridList"], _angular_material_grid_list__WEBPACK_IMPORTED_MODULE_3__["MatGridTile"], _angular_material_form_field__WEBPACK_IMPORTED_MODULE_4__["MatFormField"], _angular_material_input__WEBPACK_IMPORTED_MODULE_5__["MatInput"], _angular_common__WEBPACK_IMPORTED_MODULE_6__["NgIf"], _angular_material_radio__WEBPACK_IMPORTED_MODULE_7__["MatRadioGroup"], _angular_material_radio__WEBPACK_IMPORTED_MODULE_7__["MatRadioButton"], _angular_material_icon__WEBPACK_IMPORTED_MODULE_8__["MatIcon"], _angular_material_button__WEBPACK_IMPORTED_MODULE_9__["MatButton"], _angular_material_form_field__WEBPACK_IMPORTED_MODULE_4__["MatError"]], styles: ["mat-form-field[_ngcontent-%COMP%] {\r\n    width: 80%;\r\n  }\r\n\r\n\r\n  .btn-sec[_ngcontent-%COMP%] {\r\n    margin-right: 3rem !important;\r\n    text-align: right;\r\n    margin-top: 1.5em;\r\n  }\r\n\r\n\r\n  .update-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\r\n\r\n\r\n  .discard-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\r\n\r\n\r\n  .text-inside[_ngcontent-%COMP%] {\r\n    position: absolute;\r\n    left: 5px;\r\n    width: 100%;\r\n  }\r\n\r\n\r\n  @media (max-width: 800px) {\r\n    .btn-sec[_ngcontent-%COMP%] {\r\n      margin-right: 1rem !important;\r\n    }\r\n  }\r\n\r\n\r\n  @media (max-width: 600px) {\r\n    .btn-sec[_ngcontent-%COMP%] {\r\n      margin-right: 1rem !important;\r\n    }\r\n  }\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbImFkZC1jb250ZW50LXByb3ZpZGVyLmNvbXBvbmVudC5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSxVQUFVO0VBQ1o7OztFQUdBO0lBQ0UsNkJBQTZCO0lBQzdCLGlCQUFpQjtJQUNqQixpQkFBaUI7RUFDbkI7OztFQUNBO0lBQ0UsV0FBVztFQUNiOzs7RUFDQTtJQUNFLFdBQVc7RUFDYjs7O0VBQ0E7SUFDRSxrQkFBa0I7SUFDbEIsU0FBUztJQUNULFdBQVc7RUFDYjs7O0VBQ0E7SUFDRTtNQUNFLDZCQUE2QjtJQUMvQjtFQUNGOzs7RUFDQTtJQUNFO01BQ0UsNkJBQTZCO0lBQy9CO0VBQ0YiLCJmaWxlIjoiYWRkLWNvbnRlbnQtcHJvdmlkZXIuY29tcG9uZW50LmNzcyIsInNvdXJjZXNDb250ZW50IjpbIm1hdC1mb3JtLWZpZWxkIHtcclxuICAgIHdpZHRoOiA4MCU7XHJcbiAgfVxyXG5cclxuXHJcbiAgLmJ0bi1zZWMge1xyXG4gICAgbWFyZ2luLXJpZ2h0OiAzcmVtICFpbXBvcnRhbnQ7XHJcbiAgICB0ZXh0LWFsaWduOiByaWdodDtcclxuICAgIG1hcmdpbi10b3A6IDEuNWVtO1xyXG4gIH1cclxuICAudXBkYXRlLWJ0biB7XHJcbiAgICBtYXJnaW46IDVweDtcclxuICB9XHJcbiAgLmRpc2NhcmQtYnRuIHtcclxuICAgIG1hcmdpbjogNXB4O1xyXG4gIH1cclxuICAudGV4dC1pbnNpZGUge1xyXG4gICAgcG9zaXRpb246IGFic29sdXRlO1xyXG4gICAgbGVmdDogNXB4O1xyXG4gICAgd2lkdGg6IDEwMCU7XHJcbiAgfVxyXG4gIEBtZWRpYSAobWF4LXdpZHRoOiA4MDBweCkge1xyXG4gICAgLmJ0bi1zZWMge1xyXG4gICAgICBtYXJnaW4tcmlnaHQ6IDFyZW0gIWltcG9ydGFudDtcclxuICAgIH1cclxuICB9XHJcbiAgQG1lZGlhIChtYXgtd2lkdGg6IDYwMHB4KSB7XHJcbiAgICAuYnRuLXNlYyB7XHJcbiAgICAgIG1hcmdpbi1yaWdodDogMXJlbSAhaW1wb3J0YW50O1xyXG4gICAgfVxyXG4gIH1cclxuICAiXX0= */"] });


/***/ }),

/***/ "Sy1n":
/*!**********************************!*\
  !*** ./src/app/app.component.ts ***!
  \**********************************/
/*! exports provided: AppComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppComponent", function() { return AppComponent; });
/* harmony import */ var _azure_msal_angular__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @azure/msal-angular */ "E8bv");
/* harmony import */ var _azure_msal_browser__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @azure/msal-browser */ "u8tD");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs */ "qCKp");
/* harmony import */ var _b2c_config__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./b2c-config */ "f7MS");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs/operators */ "kU1M");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_material_toolbar__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/toolbar */ "/t3+");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/button */ "bTqV");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/icon */ "NFeN");
/* harmony import */ var _angular_material_sidenav__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/sidenav */ "XhcP");
/* harmony import */ var _angular_material_list__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/list */ "MutI");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/common */ "ofXK");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/router */ "tyNb");
/* harmony import */ var _angular_flex_layout_extended__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! @angular/flex-layout/extended */ "znSr");















const _c0 = ["sidenav"];
function AppComponent_span_14_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "span", 12);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, "Home");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
const _c1 = function (a0) { return { "expanded": a0 }; };
const _c2 = function () { return ["content-providers"]; };
const _c3 = function () { return ["sas-key"]; };
function AppComponent_div_17_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "div", 13);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "a", 14);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2, "Content Providers");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "a", 14);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](4, "SAS Key");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r2 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("ngClass", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction1"](3, _c1, ctx_r2.showHomeSubmenu));
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("routerLink", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](5, _c2));
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("routerLink", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](6, _c3));
} }
function AppComponent_span_19_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "span", 12);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, "Content");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
const _c4 = function () { return ["unprocessed-content"]; };
const _c5 = function () { return ["processed-content"]; };
const _c6 = function () { return ["broadcasted-content"]; };
function AppComponent_div_22_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "div", 13);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "a", 14);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2, "Unprocessed");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "a", 14);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](4, "Processed");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](5, "a", 14);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](6, "Broadcasted");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r4 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("ngClass", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction1"](4, _c1, ctx_r4.showContentSubmenu));
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("routerLink", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](6, _c4));
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("routerLink", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](7, _c5));
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("routerLink", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](8, _c6));
} }
function AppComponent_span_24_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "span", 12);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, "Devices");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
const _c7 = function () { return ["devices"]; };
const _c8 = function () { return ["manage-content"]; };
function AppComponent_div_27_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "div", 13);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "a", 14);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2, "Devices");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "a", 14);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](4, "Manage content");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r6 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("ngClass", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction1"](3, _c1, ctx_r6.showDeviceSubmenu));
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("routerLink", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](5, _c7));
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("routerLink", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](6, _c8));
} }
class AppComponent {
    constructor(msalGuardConfig, authService, msalBroadcastService) {
        this.msalGuardConfig = msalGuardConfig;
        this.authService = authService;
        this.msalBroadcastService = msalBroadcastService;
        this.title = 'CMS';
        this.isIframe = false;
        this.loginDisplay = false;
        this._destroying$ = new rxjs__WEBPACK_IMPORTED_MODULE_2__["Subject"]();
        this.isExpanded = true;
        this.showHomeSubmenu = true;
        this.showContentSubmenu = true;
        this.showDeviceSubmenu = true;
    }
    // private configure() {
    //   this.oauthService.configure(authConfig);
    //   this.oauthService.tokenValidationHandler = new NullValidationHandler();
    //   this.oauthService.loadDiscoveryDocument(DiscoveryDocumentConfig.url);
    //   this.oauthService.initLoginFlow();
    // }
    // private isTokenInURL(url: string) {
    //   return url.includes("id_token") || url.includes("access_token");
    // }
    // async ngOnInit(): Promise<void> {
    //   this.userUnloadedCallback = this.onUserUnLoadedCallback(this);
    //   this.userLoadedCallback = this.onUserLoadedCallback(this);
    //   this.authService.addUserUnloadedCallback(this.userUnloadedCallback);
    //   this.authService.addUserLoadedCallback(this.userLoadedCallback);
    //   this.user = await this.authService.getUser(); 
    //   if (this.isTokenInURL(this.router.url)) {
    //       this.authService.handleCallBack(); 
    //   }
    // }
    // private onUserLoadedCallback(instance: AppComponent) {
    //   return async function (user: User, router, route) {
    //     console.log("OnUserLoadedCallback(). Got user: ");
    //     console.log(user);
    //     instance.user = user; 
    //   }
    // }
    // private onUserUnLoadedCallback(instance: AppComponent) {
    //   return async function() {
    //     console.log("OnUserUnloadedCallback().");
    //     instance.user = null;
    //   }
    // }
    // private async getUserJson() {
    //   return JSON.stringify(await this.authService.getUser()); 
    // }
    // login() {
    //   this.authService.loginRedirect().then(user => {
    //     console.log("User logged in. Name: " + user.profile.name);
    //   }); 
    // }
    // logout() {
    //   this.authService.logoutRedirect().then(); 
    // }
    ngOnInit() {
        this.isIframe = window !== window.parent && !window.opener;
        this.msalBroadcastService.inProgress$
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["filter"])((status) => status === _azure_msal_browser__WEBPACK_IMPORTED_MODULE_1__["InteractionStatus"].None), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["takeUntil"])(this._destroying$))
            .subscribe(() => {
            this.setLoginDisplay();
        });
        this.msalBroadcastService.msalSubject$
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["filter"])((msg) => msg.eventType === _azure_msal_browser__WEBPACK_IMPORTED_MODULE_1__["EventType"].LOGIN_SUCCESS || msg.eventType === _azure_msal_browser__WEBPACK_IMPORTED_MODULE_1__["EventType"].ACQUIRE_TOKEN_SUCCESS), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["takeUntil"])(this._destroying$))
            .subscribe((result) => {
            var _a;
            let payload = result.payload;
            // We need to reject id tokens that were not issued with the default sign-in policy.
            // "acr" claim in the token tells us what policy is used (NOTE: for new policies (v2.0), use "tfp" instead of "acr")
            // To learn more about b2c tokens, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/tokens-overview
            if (((_a = payload.idTokenClaims) === null || _a === void 0 ? void 0 : _a.acr) === _b2c_config__WEBPACK_IMPORTED_MODULE_3__["b2cPolicies"].names.forgotPassword) {
                window.alert('Password has been reset successfully. \nPlease sign-in with your new password.');
                return this.authService.logout();
            }
            else if (payload.idTokenClaims['acr'] === _b2c_config__WEBPACK_IMPORTED_MODULE_3__["b2cPolicies"].names.editProfile) {
                window.alert('Profile has been updated successfully. \nPlease sign-in again.');
                return this.authService.logout();
            }
            return result;
        });
        this.msalBroadcastService.msalSubject$
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["filter"])((msg) => msg.eventType === _azure_msal_browser__WEBPACK_IMPORTED_MODULE_1__["EventType"].LOGIN_FAILURE || msg.eventType === _azure_msal_browser__WEBPACK_IMPORTED_MODULE_1__["EventType"].ACQUIRE_TOKEN_FAILURE), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["takeUntil"])(this._destroying$))
            .subscribe((result) => {
            if (result.error instanceof _azure_msal_browser__WEBPACK_IMPORTED_MODULE_1__["AuthError"]) {
                // Check for forgot password error
                // Learn more about AAD error codes at https://docs.microsoft.com/azure/active-directory/develop/reference-aadsts-error-codes
                if (result.error.message.includes('AADB2C90118')) {
                    // login request with reset authority
                    let resetPasswordFlowRequest = {
                        scopes: ["openid"],
                        authority: _b2c_config__WEBPACK_IMPORTED_MODULE_3__["b2cPolicies"].authorities.forgotPassword.authority,
                    };
                    this.login(resetPasswordFlowRequest);
                }
            }
        });
    }
    setLoginDisplay() {
        this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
    }
    login(userFlowRequest) {
        if (this.msalGuardConfig.interactionType === _azure_msal_browser__WEBPACK_IMPORTED_MODULE_1__["InteractionType"].Popup) {
            if (this.msalGuardConfig.authRequest) {
                this.authService.loginPopup(Object.assign(Object.assign({}, this.msalGuardConfig.authRequest), userFlowRequest))
                    .subscribe((response) => {
                    this.authService.instance.setActiveAccount(response.account);
                });
            }
            else {
                this.authService.loginPopup(userFlowRequest)
                    .subscribe((response) => {
                    this.authService.instance.setActiveAccount(response.account);
                });
            }
        }
        else {
            if (this.msalGuardConfig.authRequest) {
                this.authService.loginRedirect(Object.assign(Object.assign({}, this.msalGuardConfig.authRequest), userFlowRequest));
            }
            else {
                this.authService.loginRedirect(userFlowRequest);
            }
        }
    }
    logout() {
        this.authService.logout();
    }
    editProfile() {
        let editProfileFlowRequest = {
            scopes: ["openid"],
            authority: _b2c_config__WEBPACK_IMPORTED_MODULE_3__["b2cPolicies"].authorities.editProfile.authority,
        };
        this.login(editProfileFlowRequest);
    }
    ngOnDestroy() {
        this._destroying$.next(undefined);
        this._destroying$.complete();
    }
}
AppComponent.ɵfac = function AppComponent_Factory(t) { return new (t || AppComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_azure_msal_angular__WEBPACK_IMPORTED_MODULE_0__["MSAL_GUARD_CONFIG"]), _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_azure_msal_angular__WEBPACK_IMPORTED_MODULE_0__["MsalService"]), _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_azure_msal_angular__WEBPACK_IMPORTED_MODULE_0__["MsalBroadcastService"])); };
AppComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdefineComponent"]({ type: AppComponent, selectors: [["app-root"]], viewQuery: function AppComponent_Query(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵviewQuery"](_c0, 1);
    } if (rf & 2) {
        let _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵloadQuery"]()) && (ctx.sidenav = _t.first);
    } }, decls: 31, vars: 6, consts: [["color", "primary", 1, "cms-toolbar"], [1, "cms-menu-spacer"], [1, "cms-app-name"], ["mat-icon-button", ""], ["autosize", "", 1, "cms-container"], ["mode", "side", "opened", "true", 1, "cms-sidenav"], ["sidenav", ""], [1, "parent"], ["class", "full-width", 4, "ngIf"], ["mat-list-icon", ""], ["class", "submenu", 3, "ngClass", 4, "ngIf"], [1, "cms-sidenav-content"], [1, "full-width"], [1, "submenu", 3, "ngClass"], ["mat-list-item", "", 3, "routerLink"]], template: function AppComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "mat-toolbar", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](1, "span", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "span");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "h1", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](4, "BlendNet CMS");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](5, "span", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](6, "button", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](7, "mat-icon");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](8, "logout");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](9, "mat-sidenav-container", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](10, "mat-sidenav", 5, 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](12, "mat-nav-list");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](13, "mat-list-item", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](14, AppComponent_span_14_Template, 2, 0, "span", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](15, "mat-icon", 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](16, "home");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](17, AppComponent_div_17_Template, 5, 7, "div", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](18, "mat-list-item", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](19, AppComponent_span_19_Template, 2, 0, "span", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](20, "mat-icon", 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](21, "local_movies");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](22, AppComponent_div_22_Template, 7, 9, "div", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](23, "mat-list-item", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](24, AppComponent_span_24_Template, 2, 0, "span", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](25, "mat-icon", 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](26, "router");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](27, AppComponent_div_27_Template, 5, 7, "div", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](28, "mat-nav-list");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](29, "div", 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](30, "router-outlet");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](14);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("ngIf", ctx.isExpanded);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("ngIf", ctx.isExpanded);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("ngIf", ctx.isExpanded);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("ngIf", ctx.isExpanded);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("ngIf", ctx.isExpanded);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("ngIf", ctx.isExpanded);
    } }, directives: [_angular_material_toolbar__WEBPACK_IMPORTED_MODULE_6__["MatToolbar"], _angular_material_button__WEBPACK_IMPORTED_MODULE_7__["MatButton"], _angular_material_icon__WEBPACK_IMPORTED_MODULE_8__["MatIcon"], _angular_material_sidenav__WEBPACK_IMPORTED_MODULE_9__["MatSidenavContainer"], _angular_material_sidenav__WEBPACK_IMPORTED_MODULE_9__["MatSidenav"], _angular_material_list__WEBPACK_IMPORTED_MODULE_10__["MatNavList"], _angular_material_list__WEBPACK_IMPORTED_MODULE_10__["MatListItem"], _angular_common__WEBPACK_IMPORTED_MODULE_11__["NgIf"], _angular_material_list__WEBPACK_IMPORTED_MODULE_10__["MatListIconCssMatStyler"], _angular_router__WEBPACK_IMPORTED_MODULE_12__["RouterOutlet"], _angular_common__WEBPACK_IMPORTED_MODULE_11__["NgClass"], _angular_flex_layout_extended__WEBPACK_IMPORTED_MODULE_13__["DefaultClassDirective"], _angular_router__WEBPACK_IMPORTED_MODULE_12__["RouterLinkWithHref"]], styles: [".cms-container[_ngcontent-%COMP%] {\r\n    height: 100%;\r\n    border: 1px solid rgba(0, 0, 0, 0.5);\r\n}\r\n.position-fixed[_ngcontent-%COMP%]{\r\n    position: fixed\r\n  }\r\n.cms-content[_ngcontent-%COMP%] {\r\n    margin-top: 70px\r\n  }\r\n.cms-sidenav-content[_ngcontent-%COMP%] {\r\n    \r\n    height: 100%;\r\n    width: 100%;\r\n    align-items: center;\r\n    justify-content: center;\r\n}\r\n.cms-sidenav[_ngcontent-%COMP%] {\r\n    -webkit-user-select: none;\r\n       -moz-user-select: none;\r\n            user-select: none;\r\n}\r\n.full-width[_ngcontent-%COMP%] {\r\n    width: 100%;\r\n}\r\n.menu-button[_ngcontent-%COMP%] {\r\n    transition: 300ms ease-in-out;\r\n    transform: rotate(0deg);\r\n}\r\n.menu-button.rotated[_ngcontent-%COMP%] {\r\n    transform: rotate(180deg);\r\n}\r\n.submenu[_ngcontent-%COMP%] {\r\n    overflow-y: hidden;\r\n    transition: transform 300ms ease;\r\n    transform: scaleY(0);\r\n    transform-origin: top;\r\n    padding-left: 30px;\r\n}\r\n.submenu.expanded[_ngcontent-%COMP%] {\r\n    transform: scaleY(1);\r\n}\r\n.cms-menu-spacer[_ngcontent-%COMP%] {\r\n    flex: 1 1 auto;\r\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbImFwcC5jb21wb25lbnQuY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0lBQ0ksWUFBWTtJQUNaLG9DQUFvQztBQUN4QztBQUNBO0lBQ0k7RUFDRjtBQUNBO0lBQ0U7RUFDRjtBQUNGO0lBQ0ksbUJBQW1CO0lBQ25CLFlBQVk7SUFDWixXQUFXO0lBQ1gsbUJBQW1CO0lBQ25CLHVCQUF1QjtBQUMzQjtBQUNBO0lBQ0kseUJBQWlCO09BQWpCLHNCQUFpQjtZQUFqQixpQkFBaUI7QUFDckI7QUFDQTtJQUNJLFdBQVc7QUFDZjtBQUNBO0lBQ0ksNkJBQTZCO0lBQzdCLHVCQUF1QjtBQUMzQjtBQUNBO0lBQ0kseUJBQXlCO0FBQzdCO0FBQ0E7SUFDSSxrQkFBa0I7SUFDbEIsZ0NBQWdDO0lBQ2hDLG9CQUFvQjtJQUNwQixxQkFBcUI7SUFDckIsa0JBQWtCO0FBQ3RCO0FBQ0E7SUFDSSxvQkFBb0I7QUFDeEI7QUFDQTtJQUNJLGNBQWM7QUFDbEIiLCJmaWxlIjoiYXBwLmNvbXBvbmVudC5jc3MiLCJzb3VyY2VzQ29udGVudCI6WyIuY21zLWNvbnRhaW5lciB7XHJcbiAgICBoZWlnaHQ6IDEwMCU7XHJcbiAgICBib3JkZXI6IDFweCBzb2xpZCByZ2JhKDAsIDAsIDAsIDAuNSk7XHJcbn1cclxuLnBvc2l0aW9uLWZpeGVke1xyXG4gICAgcG9zaXRpb246IGZpeGVkXHJcbiAgfVxyXG4gIC5jbXMtY29udGVudCB7XHJcbiAgICBtYXJnaW4tdG9wOiA3MHB4XHJcbiAgfVxyXG4uY21zLXNpZGVuYXYtY29udGVudCB7XHJcbiAgICAvKiBkaXNwbGF5OiBmbGV4OyAqL1xyXG4gICAgaGVpZ2h0OiAxMDAlO1xyXG4gICAgd2lkdGg6IDEwMCU7XHJcbiAgICBhbGlnbi1pdGVtczogY2VudGVyO1xyXG4gICAganVzdGlmeS1jb250ZW50OiBjZW50ZXI7XHJcbn1cclxuLmNtcy1zaWRlbmF2IHtcclxuICAgIHVzZXItc2VsZWN0OiBub25lO1xyXG59XHJcbi5mdWxsLXdpZHRoIHtcclxuICAgIHdpZHRoOiAxMDAlO1xyXG59XHJcbi5tZW51LWJ1dHRvbiB7XHJcbiAgICB0cmFuc2l0aW9uOiAzMDBtcyBlYXNlLWluLW91dDtcclxuICAgIHRyYW5zZm9ybTogcm90YXRlKDBkZWcpO1xyXG59XHJcbi5tZW51LWJ1dHRvbi5yb3RhdGVkIHtcclxuICAgIHRyYW5zZm9ybTogcm90YXRlKDE4MGRlZyk7XHJcbn1cclxuLnN1Ym1lbnUge1xyXG4gICAgb3ZlcmZsb3cteTogaGlkZGVuO1xyXG4gICAgdHJhbnNpdGlvbjogdHJhbnNmb3JtIDMwMG1zIGVhc2U7XHJcbiAgICB0cmFuc2Zvcm06IHNjYWxlWSgwKTtcclxuICAgIHRyYW5zZm9ybS1vcmlnaW46IHRvcDtcclxuICAgIHBhZGRpbmctbGVmdDogMzBweDtcclxufVxyXG4uc3VibWVudS5leHBhbmRlZCB7XHJcbiAgICB0cmFuc2Zvcm06IHNjYWxlWSgxKTtcclxufVxyXG4uY21zLW1lbnUtc3BhY2VyIHtcclxuICAgIGZsZXg6IDEgMSBhdXRvO1xyXG59Il19 */"] });


/***/ }),

/***/ "ZAI4":
/*!*******************************!*\
  !*** ./src/app/app.module.ts ***!
  \*******************************/
/*! exports provided: loggerCallback, MSALInstanceFactory, MSALInterceptorConfigFactory, MSALGuardConfigFactory, AppModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "loggerCallback", function() { return loggerCallback; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MSALInstanceFactory", function() { return MSALInstanceFactory; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MSALInterceptorConfigFactory", function() { return MSALInterceptorConfigFactory; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MSALGuardConfigFactory", function() { return MSALGuardConfigFactory; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppModule", function() { return AppModule; });
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/platform-browser */ "jhN1");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/router */ "tyNb");
/* harmony import */ var _app_routing_module__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./app-routing.module */ "vY5A");
/* harmony import */ var _app_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./app.component */ "Sy1n");
/* harmony import */ var _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/platform-browser/animations */ "R1ws");
/* harmony import */ var _unprocessed_unprocessed_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./unprocessed/unprocessed.component */ "pngc");
/* harmony import */ var _processed_processed_component__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./processed/processed.component */ "Oj9r");
/* harmony import */ var _broadcasted_broadcasted_component__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./broadcasted/broadcasted.component */ "DGPo");
/* harmony import */ var _devices_devices_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./devices/devices.component */ "dg0d");
/* harmony import */ var _manage_content_manage_content_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./manage-content/manage-content.component */ "bGI7");
/* harmony import */ var _material_module__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./material-module */ "j5wd");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/common/http */ "tk/3");
/* harmony import */ var _content_provider_content_provider_component__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./content-provider/content-provider.component */ "tUSU");
/* harmony import */ var _sas_key_sas_key_component__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./sas-key/sas-key.component */ "mePH");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! @angular/forms */ "3Pt+");
/* harmony import */ var _angular_material_core__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! @angular/material/core */ "FKr1");
/* harmony import */ var _angular_flex_layout__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! @angular/flex-layout */ "YUcS");
/* harmony import */ var _add_content_provider_add_content_provider_component__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ./add-content-provider/add-content-provider.component */ "QTNb");
/* harmony import */ var _b2c_config__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ./b2c-config */ "f7MS");
/* harmony import */ var _azure_msal_browser__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! @azure/msal-browser */ "u8tD");
/* harmony import */ var _azure_msal_angular__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! @azure/msal-angular */ "E8bv");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! @angular/core */ "fXoL");























const isIE = window.navigator.userAgent.indexOf("MSIE ") > -1 || window.navigator.userAgent.indexOf("Trident/") > -1;
function loggerCallback(logLevel, message) {
    console.log(message);
}
function MSALInstanceFactory() {
    return new _azure_msal_browser__WEBPACK_IMPORTED_MODULE_19__["PublicClientApplication"]({
        auth: {
            clientId: '8c495645-d008-4371-9128-718b766d7f76',
            authority: _b2c_config__WEBPACK_IMPORTED_MODULE_18__["b2cPolicies"].authorities.signUpSignIn.authority,
            redirectUri: '/',
            postLogoutRedirectUri: '/',
            knownAuthorities: [_b2c_config__WEBPACK_IMPORTED_MODULE_18__["b2cPolicies"].authorityDomain]
        },
        cache: {
            cacheLocation: _azure_msal_browser__WEBPACK_IMPORTED_MODULE_19__["BrowserCacheLocation"].LocalStorage,
            storeAuthStateInCookie: isIE,
        },
        system: {
            loggerOptions: {
                loggerCallback,
                logLevel: _azure_msal_browser__WEBPACK_IMPORTED_MODULE_19__["LogLevel"].Info,
                piiLoggingEnabled: false
            }
        }
    });
}
function MSALInterceptorConfigFactory() {
    const protectedResourceMap = new Map();
    protectedResourceMap.set(_b2c_config__WEBPACK_IMPORTED_MODULE_18__["apiConfig"].uri, _b2c_config__WEBPACK_IMPORTED_MODULE_18__["apiConfig"].scopes);
    return {
        interactionType: _azure_msal_browser__WEBPACK_IMPORTED_MODULE_19__["InteractionType"].Redirect,
        protectedResourceMap,
    };
}
function MSALGuardConfigFactory() {
    return {
        interactionType: _azure_msal_browser__WEBPACK_IMPORTED_MODULE_19__["InteractionType"].Redirect,
        authRequest: {
            scopes: [..._b2c_config__WEBPACK_IMPORTED_MODULE_18__["apiConfig"].scopes],
        },
    };
}
const appRoutes = [
    { path: 'unprocessed-content', component: _unprocessed_unprocessed_component__WEBPACK_IMPORTED_MODULE_5__["UnprocessedComponent"] },
    { path: 'processed-content', component: _processed_processed_component__WEBPACK_IMPORTED_MODULE_6__["ProcessedComponent"] },
    { path: 'broadcasted-content', component: _broadcasted_broadcasted_component__WEBPACK_IMPORTED_MODULE_7__["BroadcastedComponent"] },
    { path: 'devices', component: _devices_devices_component__WEBPACK_IMPORTED_MODULE_8__["DevicesComponent"] },
    { path: 'manage-content', component: _manage_content_manage_content_component__WEBPACK_IMPORTED_MODULE_9__["ManageContentComponent"] },
    { path: 'content-providers', component: _content_provider_content_provider_component__WEBPACK_IMPORTED_MODULE_12__["ContentProviderComponent"] },
    { path: 'sas-key', component: _sas_key_sas_key_component__WEBPACK_IMPORTED_MODULE_13__["SasKeyComponent"] }
];
class AppModule {
}
AppModule.ɵmod = _angular_core__WEBPACK_IMPORTED_MODULE_21__["ɵɵdefineNgModule"]({ type: AppModule, bootstrap: [_app_component__WEBPACK_IMPORTED_MODULE_3__["AppComponent"]] });
AppModule.ɵinj = _angular_core__WEBPACK_IMPORTED_MODULE_21__["ɵɵdefineInjector"]({ factory: function AppModule_Factory(t) { return new (t || AppModule)(); }, providers: [
        {
            provide: _angular_common_http__WEBPACK_IMPORTED_MODULE_11__["HTTP_INTERCEPTORS"],
            useClass: _azure_msal_angular__WEBPACK_IMPORTED_MODULE_20__["MsalInterceptor"],
            multi: true
        },
        {
            provide: _azure_msal_angular__WEBPACK_IMPORTED_MODULE_20__["MSAL_INSTANCE"],
            useFactory: MSALInstanceFactory
        },
        {
            provide: _azure_msal_angular__WEBPACK_IMPORTED_MODULE_20__["MSAL_GUARD_CONFIG"],
            useFactory: MSALGuardConfigFactory
        },
        {
            provide: _azure_msal_angular__WEBPACK_IMPORTED_MODULE_20__["MSAL_INTERCEPTOR_CONFIG"],
            useFactory: MSALInterceptorConfigFactory
        },
        _azure_msal_angular__WEBPACK_IMPORTED_MODULE_20__["MsalService"],
        _azure_msal_angular__WEBPACK_IMPORTED_MODULE_20__["MsalGuard"],
        _azure_msal_angular__WEBPACK_IMPORTED_MODULE_20__["MsalBroadcastService"]
    ], imports: [[
            _angular_common_http__WEBPACK_IMPORTED_MODULE_11__["HttpClientModule"],
            _angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__["BrowserModule"],
            _app_routing_module__WEBPACK_IMPORTED_MODULE_2__["AppRoutingModule"],
            _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_4__["BrowserAnimationsModule"],
            _material_module__WEBPACK_IMPORTED_MODULE_10__["CMSMaterialModule"],
            _angular_forms__WEBPACK_IMPORTED_MODULE_14__["FormsModule"],
            _angular_material_core__WEBPACK_IMPORTED_MODULE_15__["MatNativeDateModule"],
            _angular_forms__WEBPACK_IMPORTED_MODULE_14__["ReactiveFormsModule"],
            _angular_flex_layout__WEBPACK_IMPORTED_MODULE_16__["FlexLayoutModule"],
            _azure_msal_angular__WEBPACK_IMPORTED_MODULE_20__["MsalModule"],
            _angular_router__WEBPACK_IMPORTED_MODULE_1__["RouterModule"].forRoot(appRoutes, { relativeLinkResolution: 'legacy' }),
        ], _angular_router__WEBPACK_IMPORTED_MODULE_1__["RouterModule"]] });
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && _angular_core__WEBPACK_IMPORTED_MODULE_21__["ɵɵsetNgModuleScope"](AppModule, { declarations: [_app_component__WEBPACK_IMPORTED_MODULE_3__["AppComponent"],
        _unprocessed_unprocessed_component__WEBPACK_IMPORTED_MODULE_5__["UnprocessedComponent"],
        _processed_processed_component__WEBPACK_IMPORTED_MODULE_6__["ProcessedComponent"],
        _broadcasted_broadcasted_component__WEBPACK_IMPORTED_MODULE_7__["BroadcastedComponent"],
        _devices_devices_component__WEBPACK_IMPORTED_MODULE_8__["DevicesComponent"],
        _manage_content_manage_content_component__WEBPACK_IMPORTED_MODULE_9__["ManageContentComponent"],
        _content_provider_content_provider_component__WEBPACK_IMPORTED_MODULE_12__["ContentProviderComponent"],
        _sas_key_sas_key_component__WEBPACK_IMPORTED_MODULE_13__["SasKeyComponent"],
        _add_content_provider_add_content_provider_component__WEBPACK_IMPORTED_MODULE_17__["AddContentProviderComponent"]], imports: [_angular_common_http__WEBPACK_IMPORTED_MODULE_11__["HttpClientModule"],
        _angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__["BrowserModule"],
        _app_routing_module__WEBPACK_IMPORTED_MODULE_2__["AppRoutingModule"],
        _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_4__["BrowserAnimationsModule"],
        _material_module__WEBPACK_IMPORTED_MODULE_10__["CMSMaterialModule"],
        _angular_forms__WEBPACK_IMPORTED_MODULE_14__["FormsModule"],
        _angular_material_core__WEBPACK_IMPORTED_MODULE_15__["MatNativeDateModule"],
        _angular_forms__WEBPACK_IMPORTED_MODULE_14__["ReactiveFormsModule"],
        _angular_flex_layout__WEBPACK_IMPORTED_MODULE_16__["FlexLayoutModule"],
        _azure_msal_angular__WEBPACK_IMPORTED_MODULE_20__["MsalModule"], _angular_router__WEBPACK_IMPORTED_MODULE_1__["RouterModule"]], exports: [_angular_router__WEBPACK_IMPORTED_MODULE_1__["RouterModule"]] }); })();


/***/ }),

/***/ "bGI7":
/*!************************************************************!*\
  !*** ./src/app/manage-content/manage-content.component.ts ***!
  \************************************************************/
/*! exports provided: ManageContentComponent, DeviceConfirmDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ManageContentComponent", function() { return ManageContentComponent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DeviceConfirmDialog", function() { return DeviceConfirmDialog; });
/* harmony import */ var _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/cdk/collections */ "0EQZ");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "0IaG");
/* harmony import */ var _angular_material_paginator__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/paginator */ "M9IT");
/* harmony import */ var _angular_material_sort__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/sort */ "Dh3D");
/* harmony import */ var _angular_material_table__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/table */ "+0xr");
/* harmony import */ var _angular_flex_layout_flex__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/flex-layout/flex */ "XiUz");
/* harmony import */ var _angular_material_card__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/card */ "Wp6s");
/* harmony import */ var _angular_material_form_field__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/form-field */ "kmnG");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/input */ "qFsG");
/* harmony import */ var _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/checkbox */ "bSwM");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/button */ "bTqV");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/icon */ "NFeN");
/* harmony import */ var _angular_material_list__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! @angular/material/list */ "MutI");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! @angular/material/tooltip */ "Qu3c");




















function ManageContentComponent_th_16_Template(rf, ctx) { if (rf & 1) {
    const _r25 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "mat-checkbox", 20);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("change", function ManageContentComponent_th_16_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r25); const ctx_r24 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](); return $event ? ctx_r24.masterToggleNewDevices() : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r1 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("checked", ctx_r1.selection.hasValue() && ctx_r1.isAllSelectedNewDevices())("indeterminate", ctx_r1.selection.hasValue() && !ctx_r1.isAllSelectedNewDevices());
} }
function ManageContentComponent_td_17_Template(rf, ctx) { if (rf & 1) {
    const _r29 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "mat-checkbox", 22);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function ManageContentComponent_td_17_Template_mat_checkbox_click_1_listener($event) { return $event.stopPropagation(); })("change", function ManageContentComponent_td_17_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r29); const row_r26 = ctx.$implicit; const ctx_r28 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](); return $event ? ctx_r28.selection.toggle(row_r26) : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r26 = ctx.$implicit;
    const ctx_r2 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("checked", ctx_r2.selection.isSelected(row_r26));
} }
function ManageContentComponent_th_19_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1, " ID ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function ManageContentComponent_td_20_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r30 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", row_r30.id, " ");
} }
function ManageContentComponent_th_22_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1, " Device Name ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function ManageContentComponent_td_23_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r31 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", row_r31.name, " ");
} }
function ManageContentComponent_th_25_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "button", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](2, "Delete device ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function ManageContentComponent_td_26_Template(rf, ctx) { if (rf & 1) {
    const _r34 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "button", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function ManageContentComponent_td_26_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r34); const ctx_r33 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](); return ctx_r33.openDeleteConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "mat-icon", 26);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](3, "delete");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function ManageContentComponent_tr_27_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](0, "tr", 27);
} }
function ManageContentComponent_tr_28_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](0, "tr", 28);
} }
function ManageContentComponent_tr_29_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "tr", 29);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "td", 30);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"]();
    const _r0 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵreference"](11);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"]("No data matching the filter \"", _r0.value, "\"");
} }
function ManageContentComponent_th_43_Template(rf, ctx) { if (rf & 1) {
    const _r37 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "mat-checkbox", 20);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("change", function ManageContentComponent_th_43_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r37); const ctx_r36 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](); return $event ? ctx_r36.masterToggleExistingDevices() : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r13 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("checked", ctx_r13.selection.hasValue() && ctx_r13.isAllSelectedExistingDevices())("indeterminate", ctx_r13.selection.hasValue() && !ctx_r13.isAllSelectedExistingDevices());
} }
function ManageContentComponent_td_44_Template(rf, ctx) { if (rf & 1) {
    const _r41 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "mat-checkbox", 22);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function ManageContentComponent_td_44_Template_mat_checkbox_click_1_listener($event) { return $event.stopPropagation(); })("change", function ManageContentComponent_td_44_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r41); const row_r38 = ctx.$implicit; const ctx_r40 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](); return $event ? ctx_r40.selection.toggle(row_r38) : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r38 = ctx.$implicit;
    const ctx_r14 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("checked", ctx_r14.selection.isSelected(row_r38));
} }
function ManageContentComponent_th_46_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1, " ID ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function ManageContentComponent_td_47_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r42 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", row_r42.id, " ");
} }
function ManageContentComponent_th_49_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1, " Device Name ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function ManageContentComponent_td_50_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r43 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", row_r43.name, " ");
} }
function ManageContentComponent_th_52_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "button", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](2, "Add device ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function ManageContentComponent_td_53_Template(rf, ctx) { if (rf & 1) {
    const _r46 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "button", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function ManageContentComponent_td_53_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r46); const ctx_r45 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](); return ctx_r45.openDeleteConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "mat-icon", 31);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](3, "add");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function ManageContentComponent_tr_54_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](0, "tr", 27);
} }
function ManageContentComponent_tr_55_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](0, "tr", 28);
} }
function ManageContentComponent_tr_56_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "tr", 29);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "td", 30);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"]();
    const _r0 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵreference"](11);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"]("No data matching the filter \"", _r0.value, "\"");
} }
const _c0 = function () { return [5, 10, 25, 100]; };
const NAMES = [
    'Map-100-KA', 'MAP-200-MH', 'MAP-200-TN', 'MAP-200-MP', 'MAP-200-UP', 'MAP-200-BH'
];
/**
 * @title Data table with sorting, pagination, and filtering.
 */
class ManageContentComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this.displayedColumns = ['select', 'id', 'name', 'action'];
        this.fileToUpload = null;
        this.showDialog = false;
        this.message = "Please press OK to continue.";
        this.initialSelection = [];
        this.allowMultiSelect = true;
        this.selection = new _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_0__["SelectionModel"](this.allowMultiSelect, this.initialSelection);
        this.paginator = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["QueryList"]();
        this.sort = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["QueryList"]();
        // Create 100 users
        const users = Array.from({ length: 100 }, (_, k) => createNewUser(k + 1));
        // Assign the data to the data source for the table to render
        this.dataSourceExistingDevices = new _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatTableDataSource"](users);
        this.dataSourceNewDevices = new _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatTableDataSource"](users);
    }
    ngAfterViewInit() {
        this.dataSourceExistingDevices.paginator = this.paginator.toArray()[0];
        this.dataSourceExistingDevices.sort = this.sort.toArray()[0];
        this.dataSourceNewDevices.paginator = this.paginator.toArray()[1];
        this.dataSourceNewDevices.sort = this.sort.toArray()[1];
    }
    applyFilterExistingDevices(event) {
        const filterValue = event.target.value;
        this.dataSourceExistingDevices.filter = filterValue.trim().toLowerCase();
        if (this.dataSourceExistingDevices.paginator) {
            this.dataSourceExistingDevices.paginator.firstPage();
        }
    }
    applyFilterNewDevices(event) {
        const filterValue = event.target.value;
        this.dataSourceNewDevices.filter = filterValue.trim().toLowerCase();
        if (this.dataSourceNewDevices.paginator) {
            this.dataSourceNewDevices.paginator.firstPage();
        }
    }
    isAllSelectedExistingDevices() {
        const numSelected = this.selection.selected.length;
        const numRows = this.dataSourceExistingDevices.data.length;
        return numSelected == numRows;
    }
    isAllSelectedNewDevices() {
        const numSelected = this.selection.selected.length;
        const numRows = this.dataSourceNewDevices.data.length;
        return numSelected == numRows;
    }
    masterToggleExistingDevices() {
        this.isAllSelectedExistingDevices() ?
            this.selection.clear() :
            this.dataSourceExistingDevices.data.forEach(row => this.selection.select(row));
    }
    masterToggleNewDevices() {
        this.isAllSelectedNewDevices() ?
            this.selection.clear() :
            this.dataSourceNewDevices.data.forEach(row => this.selection.select(row));
    }
    openDeleteConfirmModal() {
        const dialogRef = this.dialog.open(DeviceConfirmDialog, {
            data: { message: this.message }
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
    openFiltersConfirmModal() {
        const dialogRef = this.dialog.open(DeviceConfirmDialog, {
            data: { message: this.message },
            width: '40%'
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
}
ManageContentComponent.ɵfac = function ManageContentComponent_Factory(t) { return new (t || ManageContentComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialog"])); };
ManageContentComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineComponent"]({ type: ManageContentComponent, selectors: [["app-manage-content"]], viewQuery: function ManageContentComponent_Query(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵviewQuery"](_angular_material_paginator__WEBPACK_IMPORTED_MODULE_3__["MatPaginator"], 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵviewQuery"](_angular_material_sort__WEBPACK_IMPORTED_MODULE_4__["MatSort"], 1);
    } if (rf & 2) {
        let _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵloadQuery"]()) && (ctx.paginator = _t);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵloadQuery"]()) && (ctx.sort = _t);
    } }, decls: 58, vars: 10, consts: [[1, "cms-container"], ["fxLayout", "row wrap", "fxLayoutGap", "32px", "fxLayoutAlign", "flex-start"], ["fxFlex", "0 1 calc(50% - 32px)"], ["matInput", "", "placeholder", "Ex. MAP-100-MH", 3, "keyup"], ["input", ""], [1, "mat-elevation-z8"], [1, "cms-table-container"], ["mat-table", "", "matSort", "", 3, "dataSource"], ["matColumnDef", "select"], ["mat-header-cell", "", 4, "matHeaderCellDef"], ["mat-cell", "", 4, "matCellDef"], ["matColumnDef", "id"], ["mat-header-cell", "", "mat-sort-header", "", 4, "matHeaderCellDef"], ["matColumnDef", "name"], ["matColumnDef", "action"], ["mat-header-row", "", 4, "matHeaderRowDef"], ["mat-row", "", 4, "matRowDef", "matRowDefColumns"], ["class", "mat-row", 4, "matNoDataRow"], [3, "pageSizeOptions"], ["mat-header-cell", ""], [3, "checked", "indeterminate", "change"], ["mat-cell", ""], [3, "checked", "click", "change"], ["mat-header-cell", "", "mat-sort-header", ""], ["mat-icon-button", "", "disabled", ""], ["mat-icon-button", "", 3, "click"], ["mat-list-icon", "", "matTooltip", "Delete"], ["mat-header-row", ""], ["mat-row", ""], [1, "mat-row"], ["colspan", "4", 1, "mat-cell"], ["mat-list-icon", "", "matTooltip", "Add"]], template: function ManageContentComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "h1");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](2, "Content Name");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](3, "div", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](4, "mat-card", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](5, "h3");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](6, "Delete content from existing Devices");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](7, "mat-form-field");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](8, "mat-label");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](9, "Filter");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](10, "input", 3, 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("keyup", function ManageContentComponent_Template_input_keyup_10_listener($event) { return ctx.applyFilterExistingDevices($event); });
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](12, "div", 5);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](13, "div", 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](14, "table", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](15, 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](16, ManageContentComponent_th_16_Template, 2, 2, "th", 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](17, ManageContentComponent_td_17_Template, 2, 1, "td", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](18, 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](19, ManageContentComponent_th_19_Template, 2, 0, "th", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](20, ManageContentComponent_td_20_Template, 2, 1, "td", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](21, 13);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](22, ManageContentComponent_th_22_Template, 2, 0, "th", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](23, ManageContentComponent_td_23_Template, 2, 1, "td", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](24, 14);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](25, ManageContentComponent_th_25_Template, 3, 0, "th", 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](26, ManageContentComponent_td_26_Template, 4, 0, "td", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](27, ManageContentComponent_tr_27_Template, 1, 0, "tr", 15);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](28, ManageContentComponent_tr_28_Template, 1, 0, "tr", 16);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](29, ManageContentComponent_tr_29_Template, 3, 1, "tr", 17);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](30, "mat-paginator", 18);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](31, "mat-card", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](32, "h3");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](33, "Add content to new Devices");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](34, "mat-form-field");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](35, "mat-label");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](36, "Filter");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](37, "input", 3, 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("keyup", function ManageContentComponent_Template_input_keyup_37_listener($event) { return ctx.applyFilterNewDevices($event); });
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](39, "div", 5);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](40, "div", 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](41, "table", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](42, 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](43, ManageContentComponent_th_43_Template, 2, 2, "th", 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](44, ManageContentComponent_td_44_Template, 2, 1, "td", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](45, 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](46, ManageContentComponent_th_46_Template, 2, 0, "th", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](47, ManageContentComponent_td_47_Template, 2, 1, "td", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](48, 13);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](49, ManageContentComponent_th_49_Template, 2, 0, "th", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](50, ManageContentComponent_td_50_Template, 2, 1, "td", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](51, 14);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](52, ManageContentComponent_th_52_Template, 3, 0, "th", 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](53, ManageContentComponent_td_53_Template, 4, 0, "td", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](54, ManageContentComponent_tr_54_Template, 1, 0, "tr", 15);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](55, ManageContentComponent_tr_55_Template, 1, 0, "tr", 16);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](56, ManageContentComponent_tr_56_Template, 3, 1, "tr", 17);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](57, "mat-paginator", 18);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](14);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("dataSource", ctx.dataSourceExistingDevices);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](13);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("matHeaderRowDef", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("matRowDefColumns", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](2);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("pageSizeOptions", _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵpureFunction0"](8, _c0));
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](11);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("dataSource", ctx.dataSourceNewDevices);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](13);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("matHeaderRowDef", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("matRowDefColumns", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](2);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("pageSizeOptions", _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵpureFunction0"](9, _c0));
    } }, directives: [_angular_flex_layout_flex__WEBPACK_IMPORTED_MODULE_6__["DefaultLayoutDirective"], _angular_flex_layout_flex__WEBPACK_IMPORTED_MODULE_6__["DefaultLayoutGapDirective"], _angular_flex_layout_flex__WEBPACK_IMPORTED_MODULE_6__["DefaultLayoutAlignDirective"], _angular_material_card__WEBPACK_IMPORTED_MODULE_7__["MatCard"], _angular_flex_layout_flex__WEBPACK_IMPORTED_MODULE_6__["DefaultFlexDirective"], _angular_material_form_field__WEBPACK_IMPORTED_MODULE_8__["MatFormField"], _angular_material_form_field__WEBPACK_IMPORTED_MODULE_8__["MatLabel"], _angular_material_input__WEBPACK_IMPORTED_MODULE_9__["MatInput"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatTable"], _angular_material_sort__WEBPACK_IMPORTED_MODULE_4__["MatSort"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatColumnDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatHeaderCellDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatCellDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatHeaderRowDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatRowDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatNoDataRow"], _angular_material_paginator__WEBPACK_IMPORTED_MODULE_3__["MatPaginator"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatHeaderCell"], _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_10__["MatCheckbox"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatCell"], _angular_material_sort__WEBPACK_IMPORTED_MODULE_4__["MatSortHeader"], _angular_material_button__WEBPACK_IMPORTED_MODULE_11__["MatButton"], _angular_material_icon__WEBPACK_IMPORTED_MODULE_12__["MatIcon"], _angular_material_list__WEBPACK_IMPORTED_MODULE_13__["MatListIconCssMatStyler"], _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_14__["MatTooltip"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatHeaderRow"], _angular_material_table__WEBPACK_IMPORTED_MODULE_5__["MatRow"]], styles: ["table[_ngcontent-%COMP%] {\r\n    width: 100%;\r\n  }\r\n  \r\n  .mat-form-field[_ngcontent-%COMP%] {\r\n    font-size: 14px;\r\n    width: 100%;\r\n  }\r\n  \r\n  th.mat-header-cell[_ngcontent-%COMP%]:last-of-type, td.mat-cell[_ngcontent-%COMP%]:last-of-type, td.mat-footer-cell[_ngcontent-%COMP%]:last-of-type {\r\n\tpadding-right: 0px !important\r\n}\r\n  \r\n  .cms-container[_ngcontent-%COMP%] {\r\n    height: 80%;\r\n    width: 90%;\r\n    margin-left: 5%;\r\n    margin-top: 5%;\r\n  }\r\n  \r\n  .cms-table-container[_ngcontent-%COMP%] {\r\n    width: 100%;\r\n    position: relative;\r\n    max-height: 400px;\r\n    overflow: auto;\r\n  }\r\n  \r\n  .btn-sec[_ngcontent-%COMP%] {\r\n    margin-right: 3rem !important;\r\n    text-align: right;\r\n    margin-top: 1.5em;\r\n  }\r\n  \r\n  .update-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\r\n  \r\n  .discard-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIm1hbmFnZS1jb250ZW50LmNvbXBvbmVudC5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSxXQUFXO0VBQ2I7O0VBRUE7SUFDRSxlQUFlO0lBQ2YsV0FBVztFQUNiOztFQUVGOzs7Q0FHQztBQUNEOztFQUdFO0lBQ0UsV0FBVztJQUNYLFVBQVU7SUFDVixlQUFlO0lBQ2YsY0FBYztFQUNoQjs7RUFFQTtJQUNFLFdBQVc7SUFDWCxrQkFBa0I7SUFDbEIsaUJBQWlCO0lBQ2pCLGNBQWM7RUFDaEI7O0VBQ0E7SUFDRSw2QkFBNkI7SUFDN0IsaUJBQWlCO0lBQ2pCLGlCQUFpQjtFQUNuQjs7RUFDQTtJQUNFLFdBQVc7RUFDYjs7RUFDQTtJQUNFLFdBQVc7RUFDYiIsImZpbGUiOiJtYW5hZ2UtY29udGVudC5jb21wb25lbnQuY3NzIiwic291cmNlc0NvbnRlbnQiOlsidGFibGUge1xyXG4gICAgd2lkdGg6IDEwMCU7XHJcbiAgfVxyXG4gIFxyXG4gIC5tYXQtZm9ybS1maWVsZCB7XHJcbiAgICBmb250LXNpemU6IDE0cHg7XHJcbiAgICB3aWR0aDogMTAwJTtcclxuICB9XHJcbiAgXHJcbnRoLm1hdC1oZWFkZXItY2VsbDpsYXN0LW9mLXR5cGUsXHJcbnRkLm1hdC1jZWxsOmxhc3Qtb2YtdHlwZSxcclxudGQubWF0LWZvb3Rlci1jZWxsOmxhc3Qtb2YtdHlwZSB7XHJcblx0cGFkZGluZy1yaWdodDogMHB4ICFpbXBvcnRhbnRcclxufVxyXG5cclxuICBcclxuICAuY21zLWNvbnRhaW5lciB7XHJcbiAgICBoZWlnaHQ6IDgwJTtcclxuICAgIHdpZHRoOiA5MCU7XHJcbiAgICBtYXJnaW4tbGVmdDogNSU7XHJcbiAgICBtYXJnaW4tdG9wOiA1JTtcclxuICB9XHJcblxyXG4gIC5jbXMtdGFibGUtY29udGFpbmVyIHtcclxuICAgIHdpZHRoOiAxMDAlO1xyXG4gICAgcG9zaXRpb246IHJlbGF0aXZlO1xyXG4gICAgbWF4LWhlaWdodDogNDAwcHg7XHJcbiAgICBvdmVyZmxvdzogYXV0bztcclxuICB9XHJcbiAgLmJ0bi1zZWMge1xyXG4gICAgbWFyZ2luLXJpZ2h0OiAzcmVtICFpbXBvcnRhbnQ7XHJcbiAgICB0ZXh0LWFsaWduOiByaWdodDtcclxuICAgIG1hcmdpbi10b3A6IDEuNWVtO1xyXG4gIH1cclxuICAudXBkYXRlLWJ0biB7XHJcbiAgICBtYXJnaW46IDVweDtcclxuICB9XHJcbiAgLmRpc2NhcmQtYnRuIHtcclxuICAgIG1hcmdpbjogNXB4O1xyXG4gIH0iXX0= */"] });
class DeviceConfirmDialog {
    constructor(dialogRef, data) {
        this.dialogRef = dialogRef;
        this.data = data;
    }
    onCancelUpload() {
        this.dialogRef.close();
    }
    onConfirmUpload() {
        this.dialogRef.close();
    }
}
DeviceConfirmDialog.ɵfac = function DeviceConfirmDialog_Factory(t) { return new (t || DeviceConfirmDialog)(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MAT_DIALOG_DATA"])); };
DeviceConfirmDialog.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineComponent"]({ type: DeviceConfirmDialog, selectors: [["manage-content-confirm-dialog"]], decls: 10, vars: 1, consts: [["mat-dialog-title", ""], ["mat-dialog-content", ""], ["mat-dialog-actions", "", 1, "btn-sec"], ["mat-raised-button", "", "mat-dialog-close", "", 1, "discard-btn", 3, "click"], ["mat-raised-button", "", "color", "primary", 1, "update-btn", 3, "click"]], template: function DeviceConfirmDialog_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "h2", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1, "Confirm");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "div", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](3, "p");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](5, "div", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](6, "button", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function DeviceConfirmDialog_Template_button_click_6_listener() { return ctx.onCancelUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](7, "Cancel");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](8, "button", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function DeviceConfirmDialog_Template_button_click_8_listener() { return ctx.onConfirmUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](9, "OK");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate"](ctx.data.message);
    } }, encapsulation: 2 });
/** Builds and returns a new User. */
function createNewUser(id) {
    const name = NAMES[Math.round(Math.random() * (NAMES.length - 1))];
    return {
        id: id.toString(),
        name: name,
    };
}


/***/ }),

/***/ "dg0d":
/*!**********************************************!*\
  !*** ./src/app/devices/devices.component.ts ***!
  \**********************************************/
/*! exports provided: DevicesComponent, DeviceConfirmDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DevicesComponent", function() { return DevicesComponent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DeviceConfirmDialog", function() { return DeviceConfirmDialog; });
/* harmony import */ var _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/cdk/collections */ "0EQZ");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material/dialog */ "0IaG");
/* harmony import */ var _angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/paginator */ "M9IT");
/* harmony import */ var _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/sort */ "Dh3D");
/* harmony import */ var _angular_material_table__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/table */ "+0xr");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_material_form_field__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/form-field */ "kmnG");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/input */ "qFsG");
/* harmony import */ var _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/checkbox */ "bSwM");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/button */ "bTqV");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/icon */ "NFeN");
/* harmony import */ var _angular_material_list__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/list */ "MutI");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/tooltip */ "Qu3c");

















function DevicesComponent_th_12_Template(rf, ctx) { if (rf & 1) {
    const _r17 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "mat-checkbox", 20);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("change", function DevicesComponent_th_12_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r17); const ctx_r16 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return $event ? ctx_r16.masterToggle() : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r1 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("checked", ctx_r1.selection.hasValue() && ctx_r1.isAllSelected())("indeterminate", ctx_r1.selection.hasValue() && !ctx_r1.isAllSelected());
} }
function DevicesComponent_td_13_Template(rf, ctx) { if (rf & 1) {
    const _r21 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "mat-checkbox", 22);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function DevicesComponent_td_13_Template_mat_checkbox_click_1_listener($event) { return $event.stopPropagation(); })("change", function DevicesComponent_td_13_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r21); const row_r18 = ctx.$implicit; const ctx_r20 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return $event ? ctx_r20.selection.toggle(row_r18) : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r18 = ctx.$implicit;
    const ctx_r2 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("checked", ctx_r2.selection.isSelected(row_r18));
} }
function DevicesComponent_th_15_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " ID ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function DevicesComponent_td_16_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r22 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r22.id, " ");
} }
function DevicesComponent_th_18_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " Status ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function DevicesComponent_td_19_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r23 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r23.status, " ");
} }
function DevicesComponent_th_21_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " Device Name ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function DevicesComponent_td_22_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r24 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r24.name, " ");
} }
function DevicesComponent_th_24_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2, "Apply Filters ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function DevicesComponent_td_25_Template(rf, ctx) { if (rf & 1) {
    const _r27 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function DevicesComponent_td_25_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r27); const ctx_r26 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r26.openFiltersConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 26);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "filter_alt");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r25 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("disabled", row_r25.status === "Inactive");
} }
function DevicesComponent_th_27_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2, "Delete device ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function DevicesComponent_td_28_Template(rf, ctx) { if (rf & 1) {
    const _r30 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 27);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function DevicesComponent_td_28_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r30); const ctx_r29 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r29.openDeleteConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 28);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "delete");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function DevicesComponent_tr_29_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](0, "tr", 29);
} }
function DevicesComponent_tr_30_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](0, "tr", 30);
} }
function DevicesComponent_tr_31_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "tr", 31);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "td", 32);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    const _r0 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵreference"](7);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"]("No data matching the filter \"", _r0.value, "\"");
} }
const _c0 = function () { return [5, 10, 25, 100]; };
const NAMES = [
    'Map-100-KA', 'MAP-200-MH', 'MAP-200-TN', 'MAP-200-MP', 'MAP-200-UP', 'MAP-200-BH'
];
/**
 * @title Data table with sorting, pagination, and filtering.
 */
class DevicesComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this.displayedColumns = ['select', 'id', 'name', 'status', 'filters', 'delete'];
        this.fileToUpload = null;
        this.showDialog = false;
        this.message = "Please press OK to continue.";
        this.initialSelection = [];
        this.allowMultiSelect = true;
        this.selection = new _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_0__["SelectionModel"](this.allowMultiSelect, this.initialSelection);
        // Create 100 users
        const users = Array.from({ length: 100 }, (_, k) => createNewUser(k + 1));
        // Assign the data to the data source for the table to render
        this.dataSource = new _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatTableDataSource"](users);
    }
    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }
    applyFilter(event) {
        const filterValue = event.target.value;
        this.dataSource.filter = filterValue.trim().toLowerCase();
        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
    }
    isAllSelected() {
        const numSelected = this.selection.selected.length;
        const numRows = this.dataSource.data.length;
        return numSelected == numRows;
    }
    masterToggle() {
        this.isAllSelected() ?
            this.selection.clear() :
            this.dataSource.data.forEach(row => this.selection.select(row));
    }
    openDeleteConfirmModal() {
        const dialogRef = this.dialog.open(DeviceConfirmDialog, {
            data: { message: this.message }
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
    openFiltersConfirmModal() {
        const dialogRef = this.dialog.open(DeviceConfirmDialog, {
            data: { message: this.message },
            width: '40%'
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
}
DevicesComponent.ɵfac = function DevicesComponent_Factory(t) { return new (t || DevicesComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MatDialog"])); };
DevicesComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdefineComponent"]({ type: DevicesComponent, selectors: [["app-devices"]], viewQuery: function DevicesComponent_Query(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵviewQuery"](_angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__["MatPaginator"], 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵviewQuery"](_angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSort"], 1);
    } if (rf & 2) {
        let _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵloadQuery"]()) && (ctx.paginator = _t.first);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵloadQuery"]()) && (ctx.sort = _t.first);
    } }, decls: 33, vars: 6, consts: [[1, "cms-container"], ["matInput", "", "placeholder", "Ex. Don", 3, "keyup"], ["input", ""], [1, "mat-elevation-z8"], [1, "cms-table-container"], ["mat-table", "", "matSort", "", 1, "cms-table", 3, "dataSource"], ["matColumnDef", "select"], ["mat-header-cell", "", 4, "matHeaderCellDef"], ["mat-cell", "", 4, "matCellDef"], ["matColumnDef", "id"], ["mat-header-cell", "", "mat-sort-header", "", 4, "matHeaderCellDef"], ["matColumnDef", "status"], ["matColumnDef", "name"], ["matColumnDef", "filters"], ["matColumnDef", "delete"], ["mat-header-row", "", 4, "matHeaderRowDef", "matHeaderRowDefSticky"], ["mat-row", "", 4, "matRowDef", "matRowDefColumns"], ["class", "mat-row", 4, "matNoDataRow"], [3, "pageSizeOptions"], ["mat-header-cell", ""], [3, "checked", "indeterminate", "change"], ["mat-cell", ""], [3, "checked", "click", "change"], ["mat-header-cell", "", "mat-sort-header", ""], ["mat-icon-button", "", "disabled", ""], ["mat-icon-button", "", 3, "disabled", "click"], ["mat-list-icon", "", "matTooltip", "Apply filters"], ["mat-icon-button", "", 3, "click"], ["mat-list-icon", "", "matTooltip", "Delete device"], ["mat-header-row", ""], ["mat-row", ""], [1, "mat-row"], ["colspan", "4", 1, "mat-cell"]], template: function DevicesComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "h1");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2, "Devices");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "mat-form-field");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](4, "mat-label");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](5, "Filter");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](6, "input", 1, 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("keyup", function DevicesComponent_Template_input_keyup_6_listener($event) { return ctx.applyFilter($event); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](8, "div", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](9, "div", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](10, "table", 5);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](11, 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](12, DevicesComponent_th_12_Template, 2, 2, "th", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](13, DevicesComponent_td_13_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](14, 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](15, DevicesComponent_th_15_Template, 2, 0, "th", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](16, DevicesComponent_td_16_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](17, 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](18, DevicesComponent_th_18_Template, 2, 0, "th", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](19, DevicesComponent_td_19_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](20, 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](21, DevicesComponent_th_21_Template, 2, 0, "th", 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](22, DevicesComponent_td_22_Template, 2, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](23, 13);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](24, DevicesComponent_th_24_Template, 3, 0, "th", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](25, DevicesComponent_td_25_Template, 4, 1, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](26, 14);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](27, DevicesComponent_th_27_Template, 3, 0, "th", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](28, DevicesComponent_td_28_Template, 4, 0, "td", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](29, DevicesComponent_tr_29_Template, 1, 0, "tr", 15);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](30, DevicesComponent_tr_30_Template, 1, 0, "tr", 16);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](31, DevicesComponent_tr_31_Template, 3, 1, "tr", 17);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](32, "mat-paginator", 18);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("dataSource", ctx.dataSource);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](19);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("matHeaderRowDef", ctx.displayedColumns)("matHeaderRowDefSticky", true);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("matRowDefColumns", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("pageSizeOptions", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](5, _c0));
    } }, directives: [_angular_material_form_field__WEBPACK_IMPORTED_MODULE_6__["MatFormField"], _angular_material_form_field__WEBPACK_IMPORTED_MODULE_6__["MatLabel"], _angular_material_input__WEBPACK_IMPORTED_MODULE_7__["MatInput"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatTable"], _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSort"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatColumnDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderCellDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatCellDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderRowDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatRowDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatNoDataRow"], _angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__["MatPaginator"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderCell"], _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_8__["MatCheckbox"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatCell"], _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSortHeader"], _angular_material_button__WEBPACK_IMPORTED_MODULE_9__["MatButton"], _angular_material_icon__WEBPACK_IMPORTED_MODULE_10__["MatIcon"], _angular_material_list__WEBPACK_IMPORTED_MODULE_11__["MatListIconCssMatStyler"], _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_12__["MatTooltip"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderRow"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatRow"]], styles: ["table[_ngcontent-%COMP%] {\r\n    width: 100%;\r\n  }\r\n.cms-table[_ngcontent-%COMP%]{\r\n    height: 50%;\r\n}\r\n.mat-form-field[_ngcontent-%COMP%] {\r\n    font-size: 14px;\r\n    width: 100%;\r\n  }\r\nth.mat-header-cell[_ngcontent-%COMP%]:last-of-type, td.mat-cell[_ngcontent-%COMP%]:last-of-type, td.mat-footer-cell[_ngcontent-%COMP%]:last-of-type {\r\n\tpadding-right: 0px !important\r\n}\r\n.cms-container[_ngcontent-%COMP%] {\r\n    height: 80%;\r\n    width: 80%;\r\n    margin-left: 10%;\r\n    margin-top: 5%;\r\n  }\r\n.cms-table-container[_ngcontent-%COMP%] {\r\n    position: relative;\r\n    max-height: 400px;\r\n    overflow: auto;\r\n  }\r\n.btn-sec[_ngcontent-%COMP%] {\r\n    margin-right: 3rem !important;\r\n    text-align: right;\r\n    margin-top: 1.5em;\r\n  }\r\n.update-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\r\n.discard-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbImRldmljZXMuY29tcG9uZW50LmNzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtJQUNJLFdBQVc7RUFDYjtBQUNGO0lBQ0ksV0FBVztBQUNmO0FBQ0U7SUFDRSxlQUFlO0lBQ2YsV0FBVztFQUNiO0FBRUY7OztDQUdDO0FBQ0Q7QUFHRTtJQUNFLFdBQVc7SUFDWCxVQUFVO0lBQ1YsZ0JBQWdCO0lBQ2hCLGNBQWM7RUFDaEI7QUFFQTtJQUNFLGtCQUFrQjtJQUNsQixpQkFBaUI7SUFDakIsY0FBYztFQUNoQjtBQUNBO0lBQ0UsNkJBQTZCO0lBQzdCLGlCQUFpQjtJQUNqQixpQkFBaUI7RUFDbkI7QUFDQTtJQUNFLFdBQVc7RUFDYjtBQUNBO0lBQ0UsV0FBVztFQUNiIiwiZmlsZSI6ImRldmljZXMuY29tcG9uZW50LmNzcyIsInNvdXJjZXNDb250ZW50IjpbInRhYmxlIHtcclxuICAgIHdpZHRoOiAxMDAlO1xyXG4gIH1cclxuLmNtcy10YWJsZXtcclxuICAgIGhlaWdodDogNTAlO1xyXG59XHJcbiAgLm1hdC1mb3JtLWZpZWxkIHtcclxuICAgIGZvbnQtc2l6ZTogMTRweDtcclxuICAgIHdpZHRoOiAxMDAlO1xyXG4gIH1cclxuICBcclxudGgubWF0LWhlYWRlci1jZWxsOmxhc3Qtb2YtdHlwZSxcclxudGQubWF0LWNlbGw6bGFzdC1vZi10eXBlLFxyXG50ZC5tYXQtZm9vdGVyLWNlbGw6bGFzdC1vZi10eXBlIHtcclxuXHRwYWRkaW5nLXJpZ2h0OiAwcHggIWltcG9ydGFudFxyXG59XHJcblxyXG4gIFxyXG4gIC5jbXMtY29udGFpbmVyIHtcclxuICAgIGhlaWdodDogODAlO1xyXG4gICAgd2lkdGg6IDgwJTtcclxuICAgIG1hcmdpbi1sZWZ0OiAxMCU7XHJcbiAgICBtYXJnaW4tdG9wOiA1JTtcclxuICB9XHJcblxyXG4gIC5jbXMtdGFibGUtY29udGFpbmVyIHtcclxuICAgIHBvc2l0aW9uOiByZWxhdGl2ZTtcclxuICAgIG1heC1oZWlnaHQ6IDQwMHB4O1xyXG4gICAgb3ZlcmZsb3c6IGF1dG87XHJcbiAgfVxyXG4gIC5idG4tc2VjIHtcclxuICAgIG1hcmdpbi1yaWdodDogM3JlbSAhaW1wb3J0YW50O1xyXG4gICAgdGV4dC1hbGlnbjogcmlnaHQ7XHJcbiAgICBtYXJnaW4tdG9wOiAxLjVlbTtcclxuICB9XHJcbiAgLnVwZGF0ZS1idG4ge1xyXG4gICAgbWFyZ2luOiA1cHg7XHJcbiAgfVxyXG4gIC5kaXNjYXJkLWJ0biB7XHJcbiAgICBtYXJnaW46IDVweDtcclxuICB9Il19 */"] });
class DeviceConfirmDialog {
    constructor(dialogRef, data) {
        this.dialogRef = dialogRef;
        this.data = data;
    }
    onCancelUpload() {
        this.dialogRef.close();
    }
    onConfirmUpload() {
        this.dialogRef.close();
    }
}
DeviceConfirmDialog.ɵfac = function DeviceConfirmDialog_Factory(t) { return new (t || DeviceConfirmDialog)(_angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MatDialogRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MAT_DIALOG_DATA"])); };
DeviceConfirmDialog.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdefineComponent"]({ type: DeviceConfirmDialog, selectors: [["devices-confirm-dialog"]], decls: 10, vars: 1, consts: [["mat-dialog-title", ""], ["mat-dialog-content", ""], ["mat-dialog-actions", "", 1, "btn-sec"], ["mat-raised-button", "", "mat-dialog-close", "", 1, "discard-btn", 3, "click"], ["mat-raised-button", "", "color", "primary", 1, "update-btn", 3, "click"]], template: function DeviceConfirmDialog_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "h2", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, "Confirm");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "div", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "p");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](5, "div", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](6, "button", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function DeviceConfirmDialog_Template_button_click_6_listener() { return ctx.onCancelUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](7, "Cancel");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](8, "button", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function DeviceConfirmDialog_Template_button_click_8_listener() { return ctx.onConfirmUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](9, "OK");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate"](ctx.data.message);
    } }, encapsulation: 2 });
/** Builds and returns a new User. */
function createNewUser(id) {
    const name = NAMES[Math.round(Math.random() * (NAMES.length - 1))];
    const status = id % 2 === 0 ? 'Active' : 'Inactive';
    return {
        id: id.toString(),
        name: name,
        status: status,
        existingFilters: ['filter2', 'filter5', 'filter1', 'filter4', 'filter0', 'filter10',
            'filter2', 'filter5', 'filter1', 'filter4', 'filter0', 'filter10']
    };
}


/***/ }),

/***/ "f7MS":
/*!*******************************!*\
  !*** ./src/app/b2c-config.ts ***!
  \*******************************/
/*! exports provided: b2cPolicies, apiConfig */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "b2cPolicies", function() { return b2cPolicies; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "apiConfig", function() { return apiConfig; });
const b2cPolicies = {
    names: {
        signUpSignIn: "b2c_1a_signuporsigninwithphone",
        forgotPassword: "b2c_1_reset",
        editProfile: "b2c_1_edit_profile"
    },
    authorities: {
        signUpSignIn: {
            authority: "https://mishtudev.b2clogin.com/mishtudev.onmicrosoft.com/b2c_1a_signuporsigninwithphone",
        },
        forgotPassword: {
            authority: "https://mishtudev.b2clogin.com/mishtudev.onmicrosoft.com/b2c_1_reset",
        },
        editProfile: {
            authority: "https://mishtudev.b2clogin.com/mishtudev.onmicrosoft.com/b2c_1_edit_profile"
        }
    },
    authorityDomain: "mishtudev.b2clogin.com"
};
/**
 * Enter here the coordinates of your Web API and scopes for access token request
 * The current application coordinates were pre-registered in a B2C tenant.
 */
const apiConfig = {
    scopes: ['https://mishtudev.onmicrosoft.com/68fb9dd5-8d1b-4d00-9eaf-d781db510c7f/user.impersonation'],
    uri: 'https://mishtudev.onmicrosoft.com/68fb9dd5-8d1b-4d00-9eaf-d781db510c7f'
};


/***/ }),

/***/ "j5wd":
/*!************************************!*\
  !*** ./src/app/material-module.ts ***!
  \************************************/
/*! exports provided: CMSMaterialModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CMSMaterialModule", function() { return CMSMaterialModule; });
/* harmony import */ var _angular_cdk_a11y__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/cdk/a11y */ "u47x");
/* harmony import */ var _angular_cdk_clipboard__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/cdk/clipboard */ "UXJo");
/* harmony import */ var _angular_cdk_drag_drop__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/cdk/drag-drop */ "5+WD");
/* harmony import */ var _angular_cdk_portal__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/cdk/portal */ "+rOU");
/* harmony import */ var _angular_cdk_scrolling__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/cdk/scrolling */ "vxfF");
/* harmony import */ var _angular_cdk_stepper__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/cdk/stepper */ "B/XX");
/* harmony import */ var _angular_cdk_table__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/cdk/table */ "f6nW");
/* harmony import */ var _angular_cdk_tree__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/cdk/tree */ "FvrZ");
/* harmony import */ var _angular_material_autocomplete__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/autocomplete */ "/1cH");
/* harmony import */ var _angular_material_badge__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/badge */ "TU8p");
/* harmony import */ var _angular_material_bottom_sheet__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/bottom-sheet */ "2ChS");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/button */ "bTqV");
/* harmony import */ var _angular_material_button_toggle__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/button-toggle */ "jaxi");
/* harmony import */ var _angular_material_card__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! @angular/material/card */ "Wp6s");
/* harmony import */ var _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! @angular/material/checkbox */ "bSwM");
/* harmony import */ var _angular_material_chips__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! @angular/material/chips */ "A5z7");
/* harmony import */ var _angular_material_stepper__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! @angular/material/stepper */ "xHqg");
/* harmony import */ var _angular_material_datepicker__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! @angular/material/datepicker */ "iadO");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! @angular/material/dialog */ "0IaG");
/* harmony import */ var _angular_material_divider__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! @angular/material/divider */ "f0Cb");
/* harmony import */ var _angular_material_expansion__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! @angular/material/expansion */ "7EHt");
/* harmony import */ var _angular_material_grid_list__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! @angular/material/grid-list */ "zkoq");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_22__ = __webpack_require__(/*! @angular/material/icon */ "NFeN");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_23__ = __webpack_require__(/*! @angular/material/input */ "qFsG");
/* harmony import */ var _angular_material_list__WEBPACK_IMPORTED_MODULE_24__ = __webpack_require__(/*! @angular/material/list */ "MutI");
/* harmony import */ var _angular_material_menu__WEBPACK_IMPORTED_MODULE_25__ = __webpack_require__(/*! @angular/material/menu */ "STbY");
/* harmony import */ var _angular_material_core__WEBPACK_IMPORTED_MODULE_26__ = __webpack_require__(/*! @angular/material/core */ "FKr1");
/* harmony import */ var _angular_material_paginator__WEBPACK_IMPORTED_MODULE_27__ = __webpack_require__(/*! @angular/material/paginator */ "M9IT");
/* harmony import */ var _angular_material_progress_bar__WEBPACK_IMPORTED_MODULE_28__ = __webpack_require__(/*! @angular/material/progress-bar */ "bv9b");
/* harmony import */ var _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_29__ = __webpack_require__(/*! @angular/material/progress-spinner */ "Xa2L");
/* harmony import */ var _angular_material_radio__WEBPACK_IMPORTED_MODULE_30__ = __webpack_require__(/*! @angular/material/radio */ "QibW");
/* harmony import */ var _angular_material_select__WEBPACK_IMPORTED_MODULE_31__ = __webpack_require__(/*! @angular/material/select */ "d3UM");
/* harmony import */ var _angular_material_sidenav__WEBPACK_IMPORTED_MODULE_32__ = __webpack_require__(/*! @angular/material/sidenav */ "XhcP");
/* harmony import */ var _angular_material_slider__WEBPACK_IMPORTED_MODULE_33__ = __webpack_require__(/*! @angular/material/slider */ "5RNC");
/* harmony import */ var _angular_material_slide_toggle__WEBPACK_IMPORTED_MODULE_34__ = __webpack_require__(/*! @angular/material/slide-toggle */ "1jcm");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_35__ = __webpack_require__(/*! @angular/material/snack-bar */ "dNgK");
/* harmony import */ var _angular_material_sort__WEBPACK_IMPORTED_MODULE_36__ = __webpack_require__(/*! @angular/material/sort */ "Dh3D");
/* harmony import */ var _angular_material_table__WEBPACK_IMPORTED_MODULE_37__ = __webpack_require__(/*! @angular/material/table */ "+0xr");
/* harmony import */ var _angular_material_tabs__WEBPACK_IMPORTED_MODULE_38__ = __webpack_require__(/*! @angular/material/tabs */ "wZkO");
/* harmony import */ var _angular_material_toolbar__WEBPACK_IMPORTED_MODULE_39__ = __webpack_require__(/*! @angular/material/toolbar */ "/t3+");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_40__ = __webpack_require__(/*! @angular/material/tooltip */ "Qu3c");
/* harmony import */ var _angular_material_tree__WEBPACK_IMPORTED_MODULE_41__ = __webpack_require__(/*! @angular/material/tree */ "8yBR");
/* harmony import */ var _angular_cdk_overlay__WEBPACK_IMPORTED_MODULE_42__ = __webpack_require__(/*! @angular/cdk/overlay */ "rDax");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_43__ = __webpack_require__(/*! @angular/core */ "fXoL");












































class CMSMaterialModule {
}
CMSMaterialModule.ɵmod = _angular_core__WEBPACK_IMPORTED_MODULE_43__["ɵɵdefineNgModule"]({ type: CMSMaterialModule });
CMSMaterialModule.ɵinj = _angular_core__WEBPACK_IMPORTED_MODULE_43__["ɵɵdefineInjector"]({ factory: function CMSMaterialModule_Factory(t) { return new (t || CMSMaterialModule)(); }, imports: [_angular_cdk_a11y__WEBPACK_IMPORTED_MODULE_0__["A11yModule"],
        _angular_cdk_clipboard__WEBPACK_IMPORTED_MODULE_1__["ClipboardModule"],
        _angular_cdk_stepper__WEBPACK_IMPORTED_MODULE_5__["CdkStepperModule"],
        _angular_cdk_table__WEBPACK_IMPORTED_MODULE_6__["CdkTableModule"],
        _angular_cdk_tree__WEBPACK_IMPORTED_MODULE_7__["CdkTreeModule"],
        _angular_cdk_drag_drop__WEBPACK_IMPORTED_MODULE_2__["DragDropModule"],
        _angular_material_autocomplete__WEBPACK_IMPORTED_MODULE_8__["MatAutocompleteModule"],
        _angular_material_badge__WEBPACK_IMPORTED_MODULE_9__["MatBadgeModule"],
        _angular_material_bottom_sheet__WEBPACK_IMPORTED_MODULE_10__["MatBottomSheetModule"],
        _angular_material_button__WEBPACK_IMPORTED_MODULE_11__["MatButtonModule"],
        _angular_material_button_toggle__WEBPACK_IMPORTED_MODULE_12__["MatButtonToggleModule"],
        _angular_material_card__WEBPACK_IMPORTED_MODULE_13__["MatCardModule"],
        _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_14__["MatCheckboxModule"],
        _angular_material_chips__WEBPACK_IMPORTED_MODULE_15__["MatChipsModule"],
        _angular_material_stepper__WEBPACK_IMPORTED_MODULE_16__["MatStepperModule"],
        _angular_material_datepicker__WEBPACK_IMPORTED_MODULE_17__["MatDatepickerModule"],
        _angular_material_dialog__WEBPACK_IMPORTED_MODULE_18__["MatDialogModule"],
        _angular_material_divider__WEBPACK_IMPORTED_MODULE_19__["MatDividerModule"],
        _angular_material_expansion__WEBPACK_IMPORTED_MODULE_20__["MatExpansionModule"],
        _angular_material_grid_list__WEBPACK_IMPORTED_MODULE_21__["MatGridListModule"],
        _angular_material_icon__WEBPACK_IMPORTED_MODULE_22__["MatIconModule"],
        _angular_material_input__WEBPACK_IMPORTED_MODULE_23__["MatInputModule"],
        _angular_material_list__WEBPACK_IMPORTED_MODULE_24__["MatListModule"],
        _angular_material_menu__WEBPACK_IMPORTED_MODULE_25__["MatMenuModule"],
        _angular_material_core__WEBPACK_IMPORTED_MODULE_26__["MatNativeDateModule"],
        _angular_material_paginator__WEBPACK_IMPORTED_MODULE_27__["MatPaginatorModule"],
        _angular_material_progress_bar__WEBPACK_IMPORTED_MODULE_28__["MatProgressBarModule"],
        _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_29__["MatProgressSpinnerModule"],
        _angular_material_radio__WEBPACK_IMPORTED_MODULE_30__["MatRadioModule"],
        _angular_material_core__WEBPACK_IMPORTED_MODULE_26__["MatRippleModule"],
        _angular_material_select__WEBPACK_IMPORTED_MODULE_31__["MatSelectModule"],
        _angular_material_sidenav__WEBPACK_IMPORTED_MODULE_32__["MatSidenavModule"],
        _angular_material_slider__WEBPACK_IMPORTED_MODULE_33__["MatSliderModule"],
        _angular_material_slide_toggle__WEBPACK_IMPORTED_MODULE_34__["MatSlideToggleModule"],
        _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_35__["MatSnackBarModule"],
        _angular_material_sort__WEBPACK_IMPORTED_MODULE_36__["MatSortModule"],
        _angular_material_table__WEBPACK_IMPORTED_MODULE_37__["MatTableModule"],
        _angular_material_tabs__WEBPACK_IMPORTED_MODULE_38__["MatTabsModule"],
        _angular_material_toolbar__WEBPACK_IMPORTED_MODULE_39__["MatToolbarModule"],
        _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_40__["MatTooltipModule"],
        _angular_material_tree__WEBPACK_IMPORTED_MODULE_41__["MatTreeModule"],
        _angular_cdk_overlay__WEBPACK_IMPORTED_MODULE_42__["OverlayModule"],
        _angular_cdk_portal__WEBPACK_IMPORTED_MODULE_3__["PortalModule"],
        _angular_cdk_scrolling__WEBPACK_IMPORTED_MODULE_4__["ScrollingModule"]] });
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && _angular_core__WEBPACK_IMPORTED_MODULE_43__["ɵɵsetNgModuleScope"](CMSMaterialModule, { exports: [_angular_cdk_a11y__WEBPACK_IMPORTED_MODULE_0__["A11yModule"],
        _angular_cdk_clipboard__WEBPACK_IMPORTED_MODULE_1__["ClipboardModule"],
        _angular_cdk_stepper__WEBPACK_IMPORTED_MODULE_5__["CdkStepperModule"],
        _angular_cdk_table__WEBPACK_IMPORTED_MODULE_6__["CdkTableModule"],
        _angular_cdk_tree__WEBPACK_IMPORTED_MODULE_7__["CdkTreeModule"],
        _angular_cdk_drag_drop__WEBPACK_IMPORTED_MODULE_2__["DragDropModule"],
        _angular_material_autocomplete__WEBPACK_IMPORTED_MODULE_8__["MatAutocompleteModule"],
        _angular_material_badge__WEBPACK_IMPORTED_MODULE_9__["MatBadgeModule"],
        _angular_material_bottom_sheet__WEBPACK_IMPORTED_MODULE_10__["MatBottomSheetModule"],
        _angular_material_button__WEBPACK_IMPORTED_MODULE_11__["MatButtonModule"],
        _angular_material_button_toggle__WEBPACK_IMPORTED_MODULE_12__["MatButtonToggleModule"],
        _angular_material_card__WEBPACK_IMPORTED_MODULE_13__["MatCardModule"],
        _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_14__["MatCheckboxModule"],
        _angular_material_chips__WEBPACK_IMPORTED_MODULE_15__["MatChipsModule"],
        _angular_material_stepper__WEBPACK_IMPORTED_MODULE_16__["MatStepperModule"],
        _angular_material_datepicker__WEBPACK_IMPORTED_MODULE_17__["MatDatepickerModule"],
        _angular_material_dialog__WEBPACK_IMPORTED_MODULE_18__["MatDialogModule"],
        _angular_material_divider__WEBPACK_IMPORTED_MODULE_19__["MatDividerModule"],
        _angular_material_expansion__WEBPACK_IMPORTED_MODULE_20__["MatExpansionModule"],
        _angular_material_grid_list__WEBPACK_IMPORTED_MODULE_21__["MatGridListModule"],
        _angular_material_icon__WEBPACK_IMPORTED_MODULE_22__["MatIconModule"],
        _angular_material_input__WEBPACK_IMPORTED_MODULE_23__["MatInputModule"],
        _angular_material_list__WEBPACK_IMPORTED_MODULE_24__["MatListModule"],
        _angular_material_menu__WEBPACK_IMPORTED_MODULE_25__["MatMenuModule"],
        _angular_material_core__WEBPACK_IMPORTED_MODULE_26__["MatNativeDateModule"],
        _angular_material_paginator__WEBPACK_IMPORTED_MODULE_27__["MatPaginatorModule"],
        _angular_material_progress_bar__WEBPACK_IMPORTED_MODULE_28__["MatProgressBarModule"],
        _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_29__["MatProgressSpinnerModule"],
        _angular_material_radio__WEBPACK_IMPORTED_MODULE_30__["MatRadioModule"],
        _angular_material_core__WEBPACK_IMPORTED_MODULE_26__["MatRippleModule"],
        _angular_material_select__WEBPACK_IMPORTED_MODULE_31__["MatSelectModule"],
        _angular_material_sidenav__WEBPACK_IMPORTED_MODULE_32__["MatSidenavModule"],
        _angular_material_slider__WEBPACK_IMPORTED_MODULE_33__["MatSliderModule"],
        _angular_material_slide_toggle__WEBPACK_IMPORTED_MODULE_34__["MatSlideToggleModule"],
        _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_35__["MatSnackBarModule"],
        _angular_material_sort__WEBPACK_IMPORTED_MODULE_36__["MatSortModule"],
        _angular_material_table__WEBPACK_IMPORTED_MODULE_37__["MatTableModule"],
        _angular_material_tabs__WEBPACK_IMPORTED_MODULE_38__["MatTabsModule"],
        _angular_material_toolbar__WEBPACK_IMPORTED_MODULE_39__["MatToolbarModule"],
        _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_40__["MatTooltipModule"],
        _angular_material_tree__WEBPACK_IMPORTED_MODULE_41__["MatTreeModule"],
        _angular_cdk_overlay__WEBPACK_IMPORTED_MODULE_42__["OverlayModule"],
        _angular_cdk_portal__WEBPACK_IMPORTED_MODULE_3__["PortalModule"],
        _angular_cdk_scrolling__WEBPACK_IMPORTED_MODULE_4__["ScrollingModule"]] }); })();


/***/ }),

/***/ "mePH":
/*!**********************************************!*\
  !*** ./src/app/sas-key/sas-key.component.ts ***!
  \**********************************************/
/*! exports provided: SasKeyComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SasKeyComponent", function() { return SasKeyComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_material_form_field__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material/form-field */ "kmnG");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/input */ "qFsG");
/* harmony import */ var _angular_cdk_text_field__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/cdk/text-field */ "ihCf");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/forms */ "3Pt+");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/button */ "bTqV");
/* harmony import */ var _angular_cdk_clipboard__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/cdk/clipboard */ "UXJo");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/icon */ "NFeN");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/tooltip */ "Qu3c");









class SasKeyComponent {
    constructor() {
        this.value = "";
    }
    ngOnInit() {
    }
}
SasKeyComponent.ɵfac = function SasKeyComponent_Factory(t) { return new (t || SasKeyComponent)(); };
SasKeyComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdefineComponent"]({ type: SasKeyComponent, selectors: [["app-sas-key"]], decls: 16, vars: 2, consts: [[1, "cms-container"], ["appearance", "fill", 1, "sas-key-text-area"], ["matInput", "", "cdkTextareaAutosize", "", "cdkAutosizeMinRows", "5", 3, "ngModel", "ngModelChange"], ["mat-icon-button", "", 3, "cdkCopyToClipboard"], ["matTooltip", "Copy to Clipboard"], [1, "sas-key-button-row"], ["mat-raised-button", "", "color", "primary"]], template: function SasKeyComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](0, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](1, "h1");
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵtext"](2, "SAS key");
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](3, "mat-form-field", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](4, "mat-label");
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵtext"](5, "SAS Key");
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](6, "textarea", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵlistener"]("ngModelChange", function SasKeyComponent_Template_textarea_ngModelChange_6_listener($event) { return ctx.value = $event; });
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](7, "button", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](8, "mat-icon", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵtext"](9, "content_copy");
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelement"](10, "br");
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](11, "div", 5);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](12, "button", 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵtext"](13, "Clear ");
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](14, "button", 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵtext"](15, "Generate");
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵadvance"](6);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵproperty"]("ngModel", ctx.value);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵproperty"]("cdkCopyToClipboard", ctx.value);
    } }, directives: [_angular_material_form_field__WEBPACK_IMPORTED_MODULE_1__["MatFormField"], _angular_material_form_field__WEBPACK_IMPORTED_MODULE_1__["MatLabel"], _angular_material_input__WEBPACK_IMPORTED_MODULE_2__["MatInput"], _angular_cdk_text_field__WEBPACK_IMPORTED_MODULE_3__["CdkTextareaAutosize"], _angular_forms__WEBPACK_IMPORTED_MODULE_4__["DefaultValueAccessor"], _angular_forms__WEBPACK_IMPORTED_MODULE_4__["NgControlStatus"], _angular_forms__WEBPACK_IMPORTED_MODULE_4__["NgModel"], _angular_material_button__WEBPACK_IMPORTED_MODULE_5__["MatButton"], _angular_cdk_clipboard__WEBPACK_IMPORTED_MODULE_6__["CdkCopyToClipboard"], _angular_material_icon__WEBPACK_IMPORTED_MODULE_7__["MatIcon"], _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_8__["MatTooltip"]], styles: [".cms-container[_ngcontent-%COMP%] {\r\n    height: 80%;\r\n    width: 80%;\r\n    margin-left: 10%;\r\n    margin-top: 5%;\r\n  }\r\n\r\n  .sas-key-button-row[_ngcontent-%COMP%] {\r\n    text-align: left;\r\n    }\r\n\r\n  .sas-key-button-row[_ngcontent-%COMP%]   .mat-button-base[_ngcontent-%COMP%] {\r\n    margin: 8px 8px 8px 0;\r\n  }\r\n\r\n  .sas-key-text-area[_ngcontent-%COMP%] {\r\n    width: 90%;\r\n  }\r\n\r\n  .btn-sec[_ngcontent-%COMP%] {\r\n    margin-right: 3rem !important;\r\n    text-align: right;\r\n    margin-top: 1.5em;\r\n  }\r\n\r\n  .update-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\r\n\r\n  .discard-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNhcy1rZXkuY29tcG9uZW50LmNzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtJQUNJLFdBQVc7SUFDWCxVQUFVO0lBQ1YsZ0JBQWdCO0lBQ2hCLGNBQWM7RUFDaEI7O0VBRUE7SUFDRSxnQkFBZ0I7SUFDaEI7O0VBRUY7SUFDRSxxQkFBcUI7RUFDdkI7O0VBQ0E7SUFDRSxVQUFVO0VBQ1o7O0VBQ0E7SUFDRSw2QkFBNkI7SUFDN0IsaUJBQWlCO0lBQ2pCLGlCQUFpQjtFQUNuQjs7RUFDQTtJQUNFLFdBQVc7RUFDYjs7RUFDQTtJQUNFLFdBQVc7RUFDYiIsImZpbGUiOiJzYXMta2V5LmNvbXBvbmVudC5jc3MiLCJzb3VyY2VzQ29udGVudCI6WyIuY21zLWNvbnRhaW5lciB7XHJcbiAgICBoZWlnaHQ6IDgwJTtcclxuICAgIHdpZHRoOiA4MCU7XHJcbiAgICBtYXJnaW4tbGVmdDogMTAlO1xyXG4gICAgbWFyZ2luLXRvcDogNSU7XHJcbiAgfVxyXG5cclxuICAuc2FzLWtleS1idXR0b24tcm93IHtcclxuICAgIHRleHQtYWxpZ246IGxlZnQ7XHJcbiAgICB9XHJcblxyXG4gIC5zYXMta2V5LWJ1dHRvbi1yb3cgLm1hdC1idXR0b24tYmFzZSB7XHJcbiAgICBtYXJnaW46IDhweCA4cHggOHB4IDA7XHJcbiAgfVxyXG4gIC5zYXMta2V5LXRleHQtYXJlYSB7XHJcbiAgICB3aWR0aDogOTAlO1xyXG4gIH1cclxuICAuYnRuLXNlYyB7XHJcbiAgICBtYXJnaW4tcmlnaHQ6IDNyZW0gIWltcG9ydGFudDtcclxuICAgIHRleHQtYWxpZ246IHJpZ2h0O1xyXG4gICAgbWFyZ2luLXRvcDogMS41ZW07XHJcbiAgfVxyXG4gIC51cGRhdGUtYnRuIHtcclxuICAgIG1hcmdpbjogNXB4O1xyXG4gIH1cclxuICAuZGlzY2FyZC1idG4ge1xyXG4gICAgbWFyZ2luOiA1cHg7XHJcbiAgfVxyXG4gICJdfQ== */"] });


/***/ }),

/***/ "pngc":
/*!******************************************************!*\
  !*** ./src/app/unprocessed/unprocessed.component.ts ***!
  \******************************************************/
/*! exports provided: UnprocessedComponent, UnprocessConfirmDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UnprocessedComponent", function() { return UnprocessedComponent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UnprocessConfirmDialog", function() { return UnprocessConfirmDialog; });
/* harmony import */ var _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/cdk/collections */ "0EQZ");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material/dialog */ "0IaG");
/* harmony import */ var _angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/paginator */ "M9IT");
/* harmony import */ var _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/sort */ "Dh3D");
/* harmony import */ var _angular_material_table__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/table */ "+0xr");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/button */ "bTqV");
/* harmony import */ var _angular_material_form_field__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/form-field */ "kmnG");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/input */ "qFsG");
/* harmony import */ var _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/checkbox */ "bSwM");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/icon */ "NFeN");
/* harmony import */ var _angular_material_list__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/list */ "MutI");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/tooltip */ "Qu3c");

















function UnprocessedComponent_th_21_Template(rf, ctx) { if (rf & 1) {
    const _r17 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "mat-checkbox", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("change", function UnprocessedComponent_th_21_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r17); const ctx_r16 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return $event ? ctx_r16.masterToggle() : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r1 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("checked", ctx_r1.selection.hasValue() && ctx_r1.isAllSelected())("indeterminate", ctx_r1.selection.hasValue() && !ctx_r1.isAllSelected());
} }
function UnprocessedComponent_td_22_Template(rf, ctx) { if (rf & 1) {
    const _r21 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "mat-checkbox", 26);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function UnprocessedComponent_td_22_Template_mat_checkbox_click_1_listener($event) { return $event.stopPropagation(); })("change", function UnprocessedComponent_td_22_Template_mat_checkbox_change_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r21); const row_r18 = ctx.$implicit; const ctx_r20 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return $event ? ctx_r20.selection.toggle(row_r18) : null; });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r18 = ctx.$implicit;
    const ctx_r2 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("checked", ctx_r2.selection.isSelected(row_r18));
} }
function UnprocessedComponent_th_24_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 27);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " ID ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function UnprocessedComponent_td_25_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r22 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r22.id, " ");
} }
function UnprocessedComponent_th_27_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 27);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " Status ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function UnprocessedComponent_td_28_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r23 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r23.status, " ");
} }
function UnprocessedComponent_th_30_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 27);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, " Content Name ");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function UnprocessedComponent_td_31_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r24 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"](" ", row_r24.name, " ");
} }
function UnprocessedComponent_th_33_Template(rf, ctx) { if (rf & 1) {
    const _r26 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 28);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function UnprocessedComponent_th_33_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r26); const ctx_r25 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r25.openProcessConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 29);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "settings");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function UnprocessedComponent_td_34_Template(rf, ctx) { if (rf & 1) {
    const _r29 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 30);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function UnprocessedComponent_td_34_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r29); const ctx_r28 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r28.openProcessConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 31);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "settings");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r27 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("disabled", !row_r27.isProcessable);
} }
function UnprocessedComponent_th_36_Template(rf, ctx) { if (rf & 1) {
    const _r31 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "th", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 28);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function UnprocessedComponent_th_36_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r31); const ctx_r30 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r30.openDeleteConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 29);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "delete");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} }
function UnprocessedComponent_td_37_Template(rf, ctx) { if (rf & 1) {
    const _r34 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "td", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "button", 30);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function UnprocessedComponent_td_37_Template_button_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵrestoreView"](_r34); const ctx_r33 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"](); return ctx_r33.openDeleteConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "mat-icon", 32);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](3, "delete");
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    const row_r32 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("disabled", !row_r32.isDeletable);
} }
function UnprocessedComponent_tr_38_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](0, "tr", 33);
} }
function UnprocessedComponent_tr_39_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](0, "tr", 34);
} }
function UnprocessedComponent_tr_40_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "tr", 35);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "td", 36);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
} if (rf & 2) {
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵnextContext"]();
    const _r0 = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵreference"](16);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate1"]("No data matching the filter \"", _r0.value, "\"");
} }
const _c0 = function () { return [5, 10, 25, 100]; };
const _c1 = "table[_ngcontent-%COMP%] {\r\n    width: 100%;\r\n  }\r\n  \r\n  .mat-form-field[_ngcontent-%COMP%] {\r\n    font-size: 14px;\r\n    width: 100%;\r\n  }\r\n  \r\n  \r\n  \r\n  th.mat-header-cell[_ngcontent-%COMP%]:last-of-type, td.mat-cell[_ngcontent-%COMP%]:last-of-type, td.mat-footer-cell[_ngcontent-%COMP%]:last-of-type {\r\n\tpadding-right: 0px !important\r\n}\r\n  \r\n  .cms-container[_ngcontent-%COMP%] {\r\n    height: 80%;\r\n    width: 80%;\r\n    margin-left: 10%;\r\n    margin-top: 5%;\r\n  }\r\n  \r\n  .cms-table-container[_ngcontent-%COMP%] {\r\n    position: relative;\r\n    max-height: 400px;\r\n    overflow: auto;\r\n  }\r\n  \r\n  .btn-sec[_ngcontent-%COMP%] {\r\n    margin-right: 3rem !important;\r\n    text-align: right;\r\n    margin-top: 1.5em;\r\n  }\r\n  \r\n  .update-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\r\n  \r\n  .discard-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInVucHJvY2Vzc2VkLmNvbXBvbmVudC5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSxXQUFXO0VBQ2I7O0VBRUE7SUFDRSxlQUFlO0lBQ2YsV0FBVztFQUNiOztFQUNGOztHQUVHOztFQUNIOzs7Q0FHQztBQUNEOztFQUdFO0lBQ0UsV0FBVztJQUNYLFVBQVU7SUFDVixnQkFBZ0I7SUFDaEIsY0FBYztFQUNoQjs7RUFFQTtJQUNFLGtCQUFrQjtJQUNsQixpQkFBaUI7SUFDakIsY0FBYztFQUNoQjs7RUFFQTtJQUNFLDZCQUE2QjtJQUM3QixpQkFBaUI7SUFDakIsaUJBQWlCO0VBQ25COztFQUNBO0lBQ0UsV0FBVztFQUNiOztFQUNBO0lBQ0UsV0FBVztFQUNiIiwiZmlsZSI6InVucHJvY2Vzc2VkLmNvbXBvbmVudC5jc3MiLCJzb3VyY2VzQ29udGVudCI6WyJ0YWJsZSB7XHJcbiAgICB3aWR0aDogMTAwJTtcclxuICB9XHJcbiAgXHJcbiAgLm1hdC1mb3JtLWZpZWxkIHtcclxuICAgIGZvbnQtc2l6ZTogMTRweDtcclxuICAgIHdpZHRoOiAxMDAlO1xyXG4gIH1cclxuLyogdGgubWF0LWhlYWRlci1jZWxsIHtcclxuICBwb3NpdGlvbjogc3RpY2t5O1xyXG59ICovXHJcbnRoLm1hdC1oZWFkZXItY2VsbDpsYXN0LW9mLXR5cGUsXHJcbnRkLm1hdC1jZWxsOmxhc3Qtb2YtdHlwZSxcclxudGQubWF0LWZvb3Rlci1jZWxsOmxhc3Qtb2YtdHlwZSB7XHJcblx0cGFkZGluZy1yaWdodDogMHB4ICFpbXBvcnRhbnRcclxufVxyXG5cclxuICBcclxuICAuY21zLWNvbnRhaW5lciB7XHJcbiAgICBoZWlnaHQ6IDgwJTtcclxuICAgIHdpZHRoOiA4MCU7XHJcbiAgICBtYXJnaW4tbGVmdDogMTAlO1xyXG4gICAgbWFyZ2luLXRvcDogNSU7XHJcbiAgfVxyXG5cclxuICAuY21zLXRhYmxlLWNvbnRhaW5lciB7XHJcbiAgICBwb3NpdGlvbjogcmVsYXRpdmU7XHJcbiAgICBtYXgtaGVpZ2h0OiA0MDBweDtcclxuICAgIG92ZXJmbG93OiBhdXRvO1xyXG4gIH1cclxuXHJcbiAgLmJ0bi1zZWMge1xyXG4gICAgbWFyZ2luLXJpZ2h0OiAzcmVtICFpbXBvcnRhbnQ7XHJcbiAgICB0ZXh0LWFsaWduOiByaWdodDtcclxuICAgIG1hcmdpbi10b3A6IDEuNWVtO1xyXG4gIH1cclxuICAudXBkYXRlLWJ0biB7XHJcbiAgICBtYXJnaW46IDVweDtcclxuICB9XHJcbiAgLmRpc2NhcmQtYnRuIHtcclxuICAgIG1hcmdpbjogNXB4O1xyXG4gIH1cclxuICAiXX0= */";
const NAMES = [
    'Dabangg', 'Bajrangi Bhaijaan', 'Don', 'RamLeela', 'Race 3', 'KingKong'
];
/**
 * @title Data table with sorting, pagination, and filtering.
 */
class UnprocessedComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this.displayedColumns = ['select', 'id', 'name', 'status', 'isProcessable', 'isDeletable'];
        this.fileToUpload = null;
        this.showDialog = false;
        this.message = "Please press OK to continue.";
        this.initialSelection = [];
        this.allowMultiSelect = true;
        this.selection = new _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_0__["SelectionModel"](this.allowMultiSelect, this.initialSelection);
        // Create 100 users
        const users = Array.from({ length: 100 }, (_, k) => createNewUser(k + 1));
        // Assign the data to the data source for the table to render
        this.dataSource = new _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatTableDataSource"](users);
    }
    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }
    applyFilter(event) {
        const filterValue = event.target.value;
        this.dataSource.filter = filterValue.trim().toLowerCase();
        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
    }
    handleFileInput(files) {
        this.fileToUpload = files.item(0);
    }
    uploadFile() {
        // this.fileUploadService.postFile(this.fileToUpload).subscribe(data => {
        //   // do something, if upload success
        //   }, error => {
        //     console.log(error);
        //   });
    }
    isAllSelected() {
        const numSelected = this.selection.selected.length;
        const numRows = this.dataSource.data.length;
        return numSelected == numRows;
    }
    masterToggle() {
        this.isAllSelected() ?
            this.selection.clear() :
            this.dataSource.data.forEach(row => this.selection.select(row));
    }
    openUploadConfirmModal() {
        const dialogRef = this.dialog.open(UnprocessConfirmDialog, {
            data: { message: this.message },
            width: '40%'
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
    openProcessConfirmModal() {
        const dialogRef = this.dialog.open(UnprocessConfirmDialog, {
            data: { message: this.message }
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
    openDeleteConfirmModal() {
        const dialogRef = this.dialog.open(UnprocessConfirmDialog, {
            data: { message: this.message },
            width: '40%'
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
}
UnprocessedComponent.ɵfac = function UnprocessedComponent_Factory(t) { return new (t || UnprocessedComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MatDialog"])); };
UnprocessedComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdefineComponent"]({ type: UnprocessedComponent, selectors: [["app-unprocessed"]], viewQuery: function UnprocessedComponent_Query(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵviewQuery"](_angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__["MatPaginator"], 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵviewQuery"](_angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSort"], 1);
    } if (rf & 2) {
        let _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵloadQuery"]()) && (ctx.paginator = _t.first);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵloadQuery"]()) && (ctx.sort = _t.first);
    } }, decls: 42, vars: 5, consts: [[1, "cms-container"], [1, "form-group"], ["for", "file"], ["type", "file", "id", "file", 3, "change"], ["mat-raised-button", "", "color", "primary", 3, "click"], ["matInput", "", "placeholder", "Ex. Don", 3, "keyup"], ["input", ""], [1, "mat-elevation-z8"], [1, "cms-table-container"], ["mat-table", "", "matSort", "", 3, "dataSource"], ["matColumnDef", "select"], ["mat-header-cell", "", 4, "matHeaderCellDef"], ["mat-cell", "", 4, "matCellDef"], ["matColumnDef", "id"], ["mat-header-cell", "", "mat-sort-header", "", 4, "matHeaderCellDef"], ["matColumnDef", "status"], ["matColumnDef", "name"], ["matColumnDef", "isProcessable"], ["matColumnDef", "isDeletable"], ["mat-header-row", "", 4, "matHeaderRowDef"], ["mat-row", "", 4, "matRowDef", "matRowDefColumns"], ["class", "mat-row", 4, "matNoDataRow"], [3, "pageSizeOptions"], ["mat-header-cell", ""], [3, "checked", "indeterminate", "change"], ["mat-cell", ""], [3, "checked", "click", "change"], ["mat-header-cell", "", "mat-sort-header", ""], ["mat-icon-button", "", 3, "click"], ["mat-list-icon", ""], ["mat-icon-button", "", 3, "disabled", "click"], ["mat-list-icon", "", "matTooltip", "Start processing content"], ["mat-list-icon", "", "matTooltip", "Delete content"], ["mat-header-row", ""], ["mat-row", ""], [1, "mat-row"], ["colspan", "4", 1, "mat-cell"]], template: function UnprocessedComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](1, "div", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "label", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "b");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](4, "Upload new content/s ");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](5, "input", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("change", function UnprocessedComponent_Template_input_change_5_listener($event) { return ctx.handleFileInput($event.target.files); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](6, "button", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function UnprocessedComponent_Template_button_click_6_listener() { return ctx.openUploadConfirmModal(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](7, "Upload");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](8, "br");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](9, "br");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](10, "h1");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](11, "Unprocessed Content");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](12, "mat-form-field");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](13, "mat-label");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](14, "Filter");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](15, "input", 5, 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("keyup", function UnprocessedComponent_Template_input_keyup_15_listener($event) { return ctx.applyFilter($event); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](17, "div", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](18, "div", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](19, "table", 9);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](20, 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](21, UnprocessedComponent_th_21_Template, 2, 2, "th", 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](22, UnprocessedComponent_td_22_Template, 2, 1, "td", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](23, 13);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](24, UnprocessedComponent_th_24_Template, 2, 0, "th", 14);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](25, UnprocessedComponent_td_25_Template, 2, 1, "td", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](26, 15);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](27, UnprocessedComponent_th_27_Template, 2, 0, "th", 14);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](28, UnprocessedComponent_td_28_Template, 2, 1, "td", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](29, 16);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](30, UnprocessedComponent_th_30_Template, 2, 0, "th", 14);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](31, UnprocessedComponent_td_31_Template, 2, 1, "td", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](32, 17);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](33, UnprocessedComponent_th_33_Template, 4, 0, "th", 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](34, UnprocessedComponent_td_34_Template, 4, 1, "td", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerStart"](35, 18);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](36, UnprocessedComponent_th_36_Template, 4, 0, "th", 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](37, UnprocessedComponent_td_37_Template, 4, 1, "td", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementContainerEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](38, UnprocessedComponent_tr_38_Template, 1, 0, "tr", 19);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](39, UnprocessedComponent_tr_39_Template, 1, 0, "tr", 20);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtemplate"](40, UnprocessedComponent_tr_40_Template, 3, 1, "tr", 21);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelement"](41, "mat-paginator", 22);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](19);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("dataSource", ctx.dataSource);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](19);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("matHeaderRowDef", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("matRowDefColumns", ctx.displayedColumns);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵproperty"]("pageSizeOptions", _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵpureFunction0"](4, _c0));
    } }, directives: [_angular_material_button__WEBPACK_IMPORTED_MODULE_6__["MatButton"], _angular_material_form_field__WEBPACK_IMPORTED_MODULE_7__["MatFormField"], _angular_material_form_field__WEBPACK_IMPORTED_MODULE_7__["MatLabel"], _angular_material_input__WEBPACK_IMPORTED_MODULE_8__["MatInput"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatTable"], _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSort"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatColumnDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderCellDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatCellDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderRowDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatRowDef"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatNoDataRow"], _angular_material_paginator__WEBPACK_IMPORTED_MODULE_2__["MatPaginator"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderCell"], _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_9__["MatCheckbox"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatCell"], _angular_material_sort__WEBPACK_IMPORTED_MODULE_3__["MatSortHeader"], _angular_material_icon__WEBPACK_IMPORTED_MODULE_10__["MatIcon"], _angular_material_list__WEBPACK_IMPORTED_MODULE_11__["MatListIconCssMatStyler"], _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_12__["MatTooltip"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatHeaderRow"], _angular_material_table__WEBPACK_IMPORTED_MODULE_4__["MatRow"]], styles: [_c1] });
class UnprocessConfirmDialog {
    constructor(dialogRef, data) {
        this.dialogRef = dialogRef;
        this.data = data;
    }
    onCancelUpload() {
        this.dialogRef.close();
    }
    onConfirmUpload() {
        this.dialogRef.close();
    }
}
UnprocessConfirmDialog.ɵfac = function UnprocessConfirmDialog_Factory(t) { return new (t || UnprocessConfirmDialog)(_angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MatDialogRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_1__["MAT_DIALOG_DATA"])); };
UnprocessConfirmDialog.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵdefineComponent"]({ type: UnprocessConfirmDialog, selectors: [["upload-confirm-dialog"]], decls: 10, vars: 1, consts: [["mat-dialog-title", ""], ["mat-dialog-content", ""], ["mat-dialog-actions", ""], ["mat-button", "", "mat-dialog-close", "", 3, "click"], ["mat-button", "", 3, "click"]], template: function UnprocessConfirmDialog_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](0, "h2", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](1, "Confirm");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](2, "div", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](3, "p");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](5, "div", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](6, "button", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function UnprocessConfirmDialog_Template_button_click_6_listener() { return ctx.onCancelUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](7, "Cancel");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementStart"](8, "button", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵlistener"]("click", function UnprocessConfirmDialog_Template_button_click_8_listener() { return ctx.onConfirmUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtext"](9, "OK");
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_5__["ɵɵtextInterpolate"](ctx.data.message);
    } }, styles: [_c1] });
/** Builds and returns a new User. */
function createNewUser(id) {
    const name = NAMES[Math.round(Math.random() * (NAMES.length - 1))];
    const status = id % 2 === 0 ? 'Unprocessed' : 'Processing';
    const isProcessableVal = status === 'Unprocessed' ? true : false;
    const isDeletableVal = status === 'Unprocessed' ? true : false;
    return {
        id: id.toString(),
        name: name,
        status: status,
        isProcessable: isProcessableVal,
        isDeletable: isDeletableVal
    };
}


/***/ }),

/***/ "tUSU":
/*!****************************************************************!*\
  !*** ./src/app/content-provider/content-provider.component.ts ***!
  \****************************************************************/
/*! exports provided: ContentProviderComponent, CPEditConfirmDialog, CPDeleteConfirmDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentProviderComponent", function() { return ContentProviderComponent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CPEditConfirmDialog", function() { return CPEditConfirmDialog; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CPDeleteConfirmDialog", function() { return CPDeleteConfirmDialog; });
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/material/dialog */ "0IaG");
/* harmony import */ var _add_content_provider_add_content_provider_component__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../add-content-provider/add-content-provider.component */ "QTNb");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_flex_layout_flex__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/flex-layout/flex */ "XiUz");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/common */ "ofXK");
/* harmony import */ var _angular_material_card__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/card */ "Wp6s");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/button */ "bTqV");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/icon */ "NFeN");
/* harmony import */ var _angular_material_list__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/list */ "MutI");










function ContentProviderComponent_mat_card_2_div_1_mat_icon_3_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](0, "mat-icon", 10);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](1, " check_circle");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
} }
function ContentProviderComponent_mat_card_2_div_1_mat_icon_4_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](0, "mat-icon", 11);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](1, " unpublished");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
} }
function ContentProviderComponent_mat_card_2_div_1_Template(rf, ctx) { if (rf & 1) {
    const _r8 = _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](0, "div");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](1, "mat-card-header");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](2, "div", 5);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtemplate"](3, ContentProviderComponent_mat_card_2_div_1_mat_icon_3_Template, 2, 0, "mat-icon", 6);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtemplate"](4, ContentProviderComponent_mat_card_2_div_1_mat_icon_4_Template, 2, 0, "mat-icon", 7);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](5, "mat-card-title");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](6);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelement"](7, "img", 8);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelement"](8, "mat-card-content");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](9, "mat-card-actions");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](10, "button", 9);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵlistener"]("click", function ContentProviderComponent_mat_card_2_div_1_Template_button_click_10_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵrestoreView"](_r8); const ctx_r7 = _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵnextContext"](2); return ctx_r7.openEditConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](11, "EDIT");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](12, "button", 9);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵlistener"]("click", function ContentProviderComponent_mat_card_2_div_1_Template_button_click_12_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵrestoreView"](_r8); const ctx_r9 = _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵnextContext"](2); return ctx_r9.openDeleteConfirmModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](13, "DELETE");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
} if (rf & 2) {
    const cp_r1 = _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵnextContext"]().$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵadvance"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵproperty"]("ngIf", cp_r1.status);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵproperty"]("ngIf", !cp_r1.status);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtextInterpolate"](cp_r1.name);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵpropertyInterpolate"]("src", cp_r1.logoUrl, _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵsanitizeUrl"]);
} }
function ContentProviderComponent_mat_card_2_div_2_Template(rf, ctx) { if (rf & 1) {
    const _r12 = _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](0, "div");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](1, "mat-card-header");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelement"](2, "div", 5);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](3, "mat-card-title");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](4, "Add new");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](5, "img", 12);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵlistener"]("click", function ContentProviderComponent_mat_card_2_div_2_Template_img_click_5_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵrestoreView"](_r12); const ctx_r11 = _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵnextContext"](2); return ctx_r11.openNewCPModal(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelement"](6, "mat-card-content");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelement"](7, "mat-card-actions");
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
} }
function ContentProviderComponent_mat_card_2_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](0, "mat-card", 3);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtemplate"](1, ContentProviderComponent_mat_card_2_div_1_Template, 14, 4, "div", 4);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtemplate"](2, ContentProviderComponent_mat_card_2_div_2_Template, 8, 0, "div", 4);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
} if (rf & 2) {
    const cp_r1 = ctx.$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵproperty"]("ngIf", cp_r1.id !== null);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵproperty"]("ngIf", cp_r1.id === null);
} }
const _c0 = ".cms-card[_ngcontent-%COMP%] {\r\n    max-width: 200px;\r\n    margin-bottom: 32px;\r\n  }\r\n  \r\n  .cms-header-image[_ngcontent-%COMP%] {\r\n    background-image: url('https://material.angular.io/assets/img/examples/shiba1.jpg');\r\n    background-size: cover;\r\n  }\r\n  \r\n  .cms-container[_ngcontent-%COMP%] {\r\n    height: 80%;\r\n    width: 80%;\r\n    margin-left: 15%;\r\n    margin-top: 5%;\r\n  }\r\n  \r\n  .cms-cp-logo[_ngcontent-%COMP%] {\r\n    max-width: 200px;\r\n    max-height: 200px;\r\n  }\r\n  \r\n  .cp-status-active[_ngcontent-%COMP%] {\r\n    color: green  !important;\r\n  }\r\n  \r\n  .cp-status-inactive[_ngcontent-%COMP%] {\r\n    color: red  !important;\r\n  }\r\n  \r\n  .btn-sec[_ngcontent-%COMP%] {\r\n    margin-right: 3rem !important;\r\n    text-align: right;\r\n    margin-top: 1.5em;\r\n  }\r\n  \r\n  .update-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\r\n  \r\n  .discard-btn[_ngcontent-%COMP%] {\r\n    margin: 5px;\r\n  }\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbImNvbnRlbnQtcHJvdmlkZXIuY29tcG9uZW50LmNzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtJQUNJLGdCQUFnQjtJQUNoQixtQkFBbUI7RUFDckI7O0VBRUE7SUFDRSxtRkFBbUY7SUFDbkYsc0JBQXNCO0VBQ3hCOztFQUdBO0lBQ0UsV0FBVztJQUNYLFVBQVU7SUFDVixnQkFBZ0I7SUFDaEIsY0FBYztFQUNoQjs7RUFFQTtJQUNFLGdCQUFnQjtJQUNoQixpQkFBaUI7RUFDbkI7O0VBQ0E7SUFDRSx3QkFBd0I7RUFDMUI7O0VBQ0E7SUFDRSxzQkFBc0I7RUFDeEI7O0VBRUE7SUFDRSw2QkFBNkI7SUFDN0IsaUJBQWlCO0lBQ2pCLGlCQUFpQjtFQUNuQjs7RUFDQTtJQUNFLFdBQVc7RUFDYjs7RUFDQTtJQUNFLFdBQVc7RUFDYiIsImZpbGUiOiJjb250ZW50LXByb3ZpZGVyLmNvbXBvbmVudC5jc3MiLCJzb3VyY2VzQ29udGVudCI6WyIuY21zLWNhcmQge1xyXG4gICAgbWF4LXdpZHRoOiAyMDBweDtcclxuICAgIG1hcmdpbi1ib3R0b206IDMycHg7XHJcbiAgfVxyXG4gIFxyXG4gIC5jbXMtaGVhZGVyLWltYWdlIHtcclxuICAgIGJhY2tncm91bmQtaW1hZ2U6IHVybCgnaHR0cHM6Ly9tYXRlcmlhbC5hbmd1bGFyLmlvL2Fzc2V0cy9pbWcvZXhhbXBsZXMvc2hpYmExLmpwZycpO1xyXG4gICAgYmFja2dyb3VuZC1zaXplOiBjb3ZlcjtcclxuICB9XHJcbiAgXHJcbiAgICBcclxuICAuY21zLWNvbnRhaW5lciB7XHJcbiAgICBoZWlnaHQ6IDgwJTtcclxuICAgIHdpZHRoOiA4MCU7XHJcbiAgICBtYXJnaW4tbGVmdDogMTUlO1xyXG4gICAgbWFyZ2luLXRvcDogNSU7XHJcbiAgfVxyXG5cclxuICAuY21zLWNwLWxvZ28ge1xyXG4gICAgbWF4LXdpZHRoOiAyMDBweDtcclxuICAgIG1heC1oZWlnaHQ6IDIwMHB4O1xyXG4gIH1cclxuICAuY3Atc3RhdHVzLWFjdGl2ZSB7XHJcbiAgICBjb2xvcjogZ3JlZW4gICFpbXBvcnRhbnQ7XHJcbiAgfVxyXG4gIC5jcC1zdGF0dXMtaW5hY3RpdmUge1xyXG4gICAgY29sb3I6IHJlZCAgIWltcG9ydGFudDtcclxuICB9XHJcblxyXG4gIC5idG4tc2VjIHtcclxuICAgIG1hcmdpbi1yaWdodDogM3JlbSAhaW1wb3J0YW50O1xyXG4gICAgdGV4dC1hbGlnbjogcmlnaHQ7XHJcbiAgICBtYXJnaW4tdG9wOiAxLjVlbTtcclxuICB9XHJcbiAgLnVwZGF0ZS1idG4ge1xyXG4gICAgbWFyZ2luOiA1cHg7XHJcbiAgfVxyXG4gIC5kaXNjYXJkLWJ0biB7XHJcbiAgICBtYXJnaW46IDVweDtcclxuICB9Il19 */";
const data = [
    {
        id: null,
        name: '',
        logoUrl: '',
        status: false
    },
    {
        id: '123-123-123',
        name: 'EROSNOW',
        logoUrl: 'data:image/jpeg;base64,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',
        status: true
    },
    {
        id: '234-234-234',
        name: 'ALT Balaji',
        logoUrl: 'data:image/jpeg;base64,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',
        status: true
    },
    {
        id: '345-345-345',
        name: 'Voot',
        logoUrl: 'data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBxMPEhUSEQ0VFhUWEBUXGA8XFRgVFRYVFRUWFhUVFRUYHSggGBomHRUVITEhJSkrLi4uGB8zODMtNygtLisBCgoKDg0OGhAQGisdHx0tLS0tLS0tLS0rLS0tLS04LS0tLTctLS0tLS0tLS0tLS0tLS03LTctLS0tLS0tLSstLf/AABEIAOEA4QMBEQACEQEDEQH/xAAbAAABBQEBAAAAAAAAAAAAAAACAAEDBQYHBP/EAEcQAAIBAgIFCQQGBwcEAwAAAAECAAMRBCEFBhIxQQcTUWFxgZGhsSIyUtEUNHJzkrJCQ2KCweHwFRYkVGSiszNFU9IjJTX/xAAcAQABBQEBAQAAAAAAAAAAAAACAAEDBAUGBwj/xAAzEQACAgIBAwIEBAUFAQEAAAAAAQIDBBESBSExE1EyM0FhFCJxgRUjJLHwBjRCUpGhQ//aAAwDAQACEQMRAD8A7hEI82NxiUUL1HCqOJ/gI6WyOy2NceUmYrSmursdnDoFHxsLsexdw75MqkYmR1aXiszuJ0nWq5vXc/vEDwEfWvBmyyLZ/FI8pMYiQhBkEh5GwhQB0PAfkMUCQ558VihTyGbenbIpSL2NiSte32RV1KhY3JkZuwhGEdIjMYkQ0dBAmEEj0aPw222fujM/wEq5NnCPbyXcLH9Wz7F6Zk72dPFaWiNoSJCNoQSI2hIIjMJBkLQ0GiJpIgkQvCQa7kmH0nWom9PEVFI6HNvA5SeFkl4ZHZiU2L88EzT6F5Rq9IhcQgqp8QGzUHfubwHbLEMqXhmPlf6frkuVL0/b6HStDaZo4xOcoVNocRuZepgcwZdhNS7o5bIxrMefGxaZZXhFcUQjzY3FrRRqjmyqLn5dsdLfgjtsjXFyl4OXab0vUxVQs2Sg+zT4KP4nrluMEkcnlZcrpt/T6FfeIqiMEIaCwkOIDDiPI2OKAx0PIwzy4vFbPsr73p/OQznrsaWJiOb5S8FaTId7NpJJaQxjhAGIJDRRCBtHckl3Cit9jQ4LDc0oHHee2Yt9vOR1OJQqq0vqSGRIuIjMIMjaEEiNoSCIzCQZEYaCRA8NBoiqQ0SRIHhokQEIc9uh9K1cJVFWkbEZFf0XHFWHEekkrscHtFXMw68mHGa/c7hq9pmnjaK1afHJl4qw3qZpwmpraPPsrGnj2OEy0hFcwWvukSzrhwclAZu07h3CWaIaWzn+rZDcvTRkZYZiikQhQQxQWOhxAYcR5GxxQB0Q4qvsjLefLrkFkuJfw8f1Jbfgq5VbN5LXgYxINDRxwDHDGiHLDRGGu22dw3dvT3SlmW8VxX1NXpmPylzl4RbGZZ0CQBhoJEZhBkbQgkRtCQRE0KIZE0MJELw0GiJ4aDRA8NEsQIQQohGs5ONMnD4oUyf/AI63snoD2Owe/d4SzjT1LRg9dw/Vp9ReY/2OyS/s4jbOQaWxHO16j9Lt4A2HkJoxWo6OMyJ87ZSPJEQikYh4IY0FjocQGHEeRscGo+yLmRTkorZNTW5zSKqoxJueMoye3s6SuChFRQEYkGjoJAmIIExw0HRpFmCjef6vAnNQi2yWmt2TUUaOlSCqFG4CYc58pbZ1dVahBRQxgonAMNDoBoQZE0IJEbQkERNCiGRNDCRE0NBoheGg0QPDRLECEEKIQVGqUYMN6sGHaDcekdPT2gLYc4OL+qOu/wB7xL3rI4r+FTMNebjPKBQRxSMQ8EMaCx0OIDCiPImEeDGVbm3Aeso3S29G5g0cY8n5Z5mkJogxxwY6CQxiCBMcMudDYWy7Z3tu6h/OZObdt8UbvTcbiucvqWBlLZsIjMJBAGGh0A0IMiaEEiNoSCImhRDImhhIiaGg0QvDRJEgeGiSIEIIUQhRCJvpL/EfGPtkfpo0AnVM+dBQRxSMQoIYoLHQ4gMKINZ9kE/1eV7ZKMS1jVOdiRVmZzZ0UVx7DRBoGOODHQSGMQR6MBhudcLwGZ7JDkWquG/qXMOh22a+hoyJhee51MVpEZjhojMJBAGGh0A0IMiaEEiNoSCImhRDImhhIiaGg0QvDQaIHholiBCCFEIUQhRCNMJ1bPnAUEcUjEPBDGgsdDiAwonkxz7h3zPyZf8AE2en16i5HklU1Bo4SBjjjGOEDE3oNLfY0ejcLzSWPvHM/wAB3fOYeTd6k/sdPhY3ow7+Wegyui8RtDQUSMwkEAYSHQDQgyJoQSI2hIIiaFEMjaGgkQtDQaIXhoNEDw0SxAhBCiEKIQohGmE6tnzgKCOKRiHghjQWOghI2HErazXJPXMmx8pNnSY8ONaRHALAMcJAxxxjHCR79DYXabbK+yvm0oZt3GPFeWanTsfnPlLwi9JmTs6IAx0OiNoaDiRmOggDDQ6AaEGRNCCRG0JBETQohkTQwkRNCQaIXkiDiQPDRLECEEKIQohCiEaYTq2fOAoI4pGIUEMUFjoTNYE9UhseosmojymkVkyTp0MYwQMcJAxxx6dMsQo3kgCBOSjFtktcHOSijT4egKahBw8zxMwLbHZLbOsoqjXBRQZgInAMJBIjaGgokZjoIAw0OgGhBkTQgkRtCQRE0KIZZ6saHGNrc2zlVCFja1zmAAL9stY1XqT0yh1HMljVqSXdm2ocn2EHvc4/a5X8tporCrOfl1rKfh6/Yw2vWiqeExPN0U2UNJWtctmSQcySeAlbIrUJaR0PR8mzIpcrHtpmYeQo24gQghRCFEIUQjTCdWz5wFBHFI2Ogtg/CfAwSRVy9mMRbepHaLQGJxcfKIsQfZP9cZWyH+Qt4Ud2o8EzDoUIUycwpPYDH0wtAuhG9SO0ERaYSAiHNBq3oeo4NRaLNfJcsrcTc5SllxtsajWto18H0q/z2M0H938Sf1H+5fnKn8OyH/xNL+I0e/8Ac82J0VXTNqDgdIsw/wBpMCWHdHzElhmUzelI8BkOmvJbT2tkbQkHEjMdMInw2jK1bOnQdh0gWH4jYSzDHtktqJXsy6a3qUj1nVfF/wCWP40+cnWFd7Ef8Uxl/wAv/jPBjND4iiLvh3A6bBh4qTAljWx8xJ687Hm9RkVrSMuJ7ACEmwBJOQAzJPUIUFthOSgts2PJ9oyrTxDu9BkXmrXYEXJYGwvv3TTw65Rlto57rOVVbXFQls6JNQ505jykaKrVcSr06Dupoqt1UtmGa4Nt28Shk1ylLaR1HQ8umqqUZy09mJxujq1EbVShUQE22mUqL9FzKzrlHyjoacum18YSTZ4oxbFEIUQhRCNMJ1bPnAUEccMQQw3ggi+64zEjY8Xp7OwYKotRFdQLMoI7xKj7HZ1uM4qSXkrNbcCKuGqWUbSLtDLP2czbtAIjxZXzaVOp9u6OWYk+z3iQ5XwGN09fzRaEwf0jEUqXx1ADx9kXZvIGUYR3JI34LbO2U6CqAAoAAsAALCaHFFgx/KbigmGWmAL1Kg6MlQbRI/2jvkNzSWgWVGoeqi1lGJxCXS55ukRk1v02HEX3DqgU1JrbHSOj06YUAAAAbgBYSykl4HJI4hRaEZ7T+gFrqXQBagBOQttdR+cz8zCjZHcV3L+HmzpklJ7iYFh/XXOecWnpnURafdGs1W1fUqK9Zb3zVCMrcGI/rhNrAw48ec0YPUM+Tk6632NgigZCayWvBjthwhhrRhGQ1t1YSojVqKBaigkqosHHHLp65QysWMk5RXc1+ndRnXNQm9x/sYzVj65Q+8HoZQxl/NWze6g/6Wf6HYpvnFBRCBMQjF8q72wQHTXTyu38JXyfgNroK/q0/szkczjutiiEKIQohGmE6tnzgKCOKRsdHRNQsbzmHKHfTYj905j5d0rWLudL0u3lTx9jSVE2gQdxEBGlJbWjjOmcNzTVE+CoR3Xy8rSPJ+A5/FhwyHEvOTTA7eIeqd1OnYdr8fAHxlbHjt7N2COmy6SnJeUfHmrijTXdSUIPttYn1A7pRue56Bfc6lgcMtKmiLuVAB3C0uRWkERaWxnMUme1yBkOknISHJu9KtzJaKvUmomJqafxBN+fI/ZCrYeInOS6hfJ7Ujfj0+hLTRpNWdMNiAyvbbW2YyBB424HKbWBlyujqXkys7FVMk4+GaCaJQOa6fwY+mOgyD1E8GAv53nN5VX9S4nTYlj/AAnL22dFpoAABuAtOiiklpHNN7eyt1j0p9FolwLsSAo4XPE9khybvShyLGJj+vaoGBfWXFk3+lEdQVLeFpi/jLm/iOlj0zGS1x/ubTVLTLYum22Bto1jbcQRcNbhNfEvdsO/lHP9QxPw9ml4ZfmWiicc0yDhsZVCHZKVSykcL2YeRmBbuFz0drhpX4sVPvtGq1I1hr4ms1OtUDAU9oeyAQQQOG/fNDFyJ2S1Ixuq4FOPBSrX1N1NAwznev2s2JwuIWlQqBF5oMfZBJJJHEHolK++cJaR0PR+m05NblYvqYjTOn8RiwFxFfaVTcDZVRfdfIZmxleV0prTZ0eL06jHlyrXf9yqMjNAaIQohCiEaadVtHzgKNtCFI20OaLUXG83idg7qqkfvLdh5XkM9Gr0u3jbx9zpMhOkOacomD2K3ODdUQHvWynytAu7w0ZN8OGUpe5oeTrA81hA531XL92Sr5C/fI6I6iakPBo8XiBSRnbIKjMT1KLyVvSC2cb0chxNdqzjI1Ns9rNcCYuVe4+PLZYxKfUk5PwjtSTaj4ICm1t+rN2r+YSj1P5DLmB89GDM5ZI6XaNJqN/1Kn2R6za6R8UjJ6s/yxNlN8xDCaf/AP0U+1S9Zg5X+8X7G9iv+il+5vJvGCZblA+rr94PQzO6l8o1Oj/P/Y56ZiJM6nsbPk3/AF37v8Zr9M+GRz3W/jj+ht5qmGcf1v8AruI+2v8AxpOfyfny/wA+h2nS/wDaw/f+5Zcmv1p/uT+ZZZwPjf6FXr3yo/qdPmucqcj5Ufrg+4T8zTNy/j/Y7D/T3yJfqYx5XR0MQI4YohCiEKIR7by9zl7nzePePzl7iFeDyfuLZPgcUaNRKg3o6t4GOpNMlpsddil7HdcNWFRFddzKGHYReWl3R2kJcoqS+pluUnRjV8OpRbutVQOx/Z8LkHugWRbXYo59UpRjKPlM0+Awwo00pruRFUdwtD0X4rS0ZzlHx3NYQoPerMEA6Qc28gRIrpKMQbNtaX1Of4OmKYA6wSeu4uZiyalPZpUxdcNHa13TeXgolDrut8I9uDKe7aEr5i3UyfHerEc0vMLivY1lJmu5OlPOVjw2FHfczS6fHTb0U85vSRvJqmcc61kP/wBmnU9H1HzmPfr8T/4a1G/wrX6nRpsGSZTlEU/RlPRVW/gZSzo7rNDpr1cc3MxtHR7NzyZKdmseG0o7wCT6zVwFqLMPqz/NH9DdTQMg49rgf8biPtr+RRMDI+fL/Podr0v/AGsP8+pZ8mv1p/uT+ZZZwPjf6FTr3yo/qdOmucqcj5Ufrg+4T8zTNzPj/Y7D/T3yJfqY15XR0MSOOGKIQohCiEWFWmVJU7wZbjJSW0fOU48XoCOAKIYcRhzrfJ7j+ewaLxpHYPYPd8iJbre0dX0yznQl7GoIhmiKIY5xr9X5zEqvClTNvtPbaP4QPEzMzbdvgiWqvb5GcI4TORdOr6Ax4xFBHvnazdTDI/11zeonzgmUJx09HuxFFailGAKsCCp3EHfJZRUlpjJtd0ZmpqPQJuKlRR8Nwbd5F/GUngwbLCy5pF5onRdLCpsUxYE3LE3LHpJlmuqNa0iGc3N7Z7KjhQSTYAXJ6hJG9LYCW+xyHTGONfEVKym16l1PQFsFPgAZgX2crHI6CivjUonVNFY1cRSSopyZb9h4jtvNuqfOKZhWQcJNMkxuESsjU6i7SsLEQpxU1pjQm4PcfJmW1CoXuKrgfDl62lP8DA0F1O1LWkaHRmjqeGpinSSyjvJPEk8TLkIKC0ijbbKyXKTJ8XiFpIzsbKqkk9QF4pSUVtgwi5yUV5ZxfSGJNaq9Q/puzeJy8pz9k+U3I7vGq9KqMPYv+TlwMWR00Wt3MDLeA16jX2M3rkW6E/udRmwcoZfWjVBMe61DVZGVbeyAbi987yCyiNj2zSwepzxE4xW0znmuurQ0eaRWoXWpcZgAhhnw4EekqW08PB0/Sepyy3KMlpozEgNsUQhRCFEI0WlMPcbY3jf2fyios12Z4DlVclyXkqpcM0UQw4jCNbyeaYTD1nSo4VKiixJsoZSbdlwfISWppb2a/SslVycZPszo39t4f/NU/wAY+cn5I3/xFf8A2QLacw4BP0mnkL+8Dl2CDKyMVtskhZGb1FnMMdiDWqvUP6bk9gJyHcLCYdsuU2y/BaR55GSosdC6ZqYRyUzU+8h3N8j1yem91vt4BnWpG4wOtuGqj2qnNnoYWH4t3nNKGZW0VnRNFkul6Bz+kU/xj5yVXQf1A9OXseTGazYWkM8QpPQt3PlBlk1pb2HHHsl4RjtYtaWxINNAUpnff3nHXbcOqZ2Rluz8q8GhRiqD5S8mcMpmii10Dp2pgibDapsbtTPT0qeB+UnoyJVPXlEGRiK5b+puMFrdhaoF6uwehgV893nNSGZXL6mTPCtj9Nlh/bWH3/Saf4x85L60Pci9Gz/qzw4vWzCU/wBeGPwqCx8shI55VUV3ZPVgX2PtExGsmsz4v2ANikD7m8sb5Fj1cAPOZmRluzsvB0OD0yNH5pd5f2M60qI1yTA4x8PVSqnvIbi+7cQQeogkd8lqnwkpIjyKI31uEvqdL0VrthawG3UFJ8ro+Qv1NuM2KsuE138nJX9JyKn2jtfYtH1gwoFzi6Vvtj5yb1I+5UWJc3pQf/hgOUbWHDYqnTp0avOMtTaJUHZAsRvIsT2StkWwlHSOi6Hg302OdkdLRgZTOpQohxRCFEI2Ilbejw1rZR43D821uBzHZ/KaNU+UTJvq9ORBJCsKMIUQgrCMF+hf6M0dakSR7T2IyzAG4TMyL9z0vB0fTK/SXJ+WQQDpYvaFHJUKMEhoiSIrREiBiJEIxBpDRiRDRyRAmINAkRiRIS74zRPX5GaMot+CxtI874lB+sXxEkVU/YfnFfUhOKT4/WGqZ+wvWh7gF1PEQ1XJBRvh7kbL+zC4smVkPdELmEmTRAjhoUQ4ohCiEbJZUkeHohx2G21/aGY+XfJabODIcin1I/dFDNLZitaFGGFEI9+isJzr5+6uZ6+gSvkW+nHt5LuHR6k+/hGpmKzpF2KzSNGzbQ3N68ZPVLa0aWPPa0eSSltHt0houpQALFSCSAVJIv3gSSypxSYUZJjYbRjvTaqCqot77RIyUXNrAxo1NrYfJJ6G/s1+Z5+67HRc7XvbO61vOL03x5Bqa5aJcDoSpXTbRktcjMm9weNgYoUuS2PK5RegsVq9XRS3sEAXIUm9h1EC8J0SS2HDIi3o8OBwNSudlFvYXJJso7TI4QcnpE8rIwW2WJ1Yrb9ulu6W/wDWSPHkgFlR34K/R+jKmIJ2ALDe5NlHVkMzBjXKRYndGHkk0hoSpRUv7L2BJRM2y6AQL+Ml/CzfgCOZDwyj0bRxGNDNhzTQKQDtsdo3F8rAi0sRxIryPPM4FdpbA1qD7FYgtshgQSykHoJA6DwkqhFeESV3Kxb2WGD1SxNZEqA0gHW4DMwax3XAU28YRFLKhF6C1J+tr93U9IweT8vsanRWtFHE1BSSk4Jv7TBNn2Rc3IYnh0R9oqTx7IR5bIq+uGHpuU5uowU2NVVUpfvIJHdnwvG2iSGJbKO9nm1xwNJqAxVO17rcrkHVyAD23IzgSimW+nZNkLPTkY0GQNaOihapDxiUUQhRCNksqHhyJBGZKim0thtltsbm8m/nL2PZyWn5MnMp4y5LwzwSyUAlUnIdNu+M3pBxjyekarR2F5pAvHee2Yt9jnI6PEp9KCR6xIGW0NVpBgVPHyPTGT4vZNXJxeymqIVJU7xLkZbRqwkpLaNDh/8AFYQpvdMh2rmviMpdj/Mr0L4ZEenX5ihToLvIF+xcye8+kG7UIqKCrXKWxf8Abx3/APKYv/xCXzSHC6JpJRFWtUYA2PsnIXNhkASTBhXFR5MklY+WkWmgxR9vmajsMtravYe9uuBvzv2CTV8e+gJuTa2eXQJIoVdj3w727bezAp+F68ktvxLfgyz1Cblieu5PneVdtvRfio+SyxekCNGMcOc7Hadd9tqzkdGXHomnTVqPcoWTTtKXk8d+dqgX2Obuejb2l2T2kbUnS0Dd47FvquAjY3YtYYhrdG4/xjjWfRMWksANJU8LWXLMbfUjWNUdoK2HaemCHXP0txLzC4wNWeku6ktO/UWDG3coXxgsBwelIw+pH1tfsVPSMaOV8oua4VdKA5ADDkm3QEa5yjARbeP29yjw61jha5oqBhjUuQ1jUsLEZ9QteCWvyc48viL7S9RG0UClwtqQF7XyqKDcjje8T8EOOmsrv9zFiRmyhwYzSLNdnuFA0WUxRhzZLKh4ciQRmSoavRFRSp4+R4GKEnGWwbK1ZDiZupTKEqd4NpqxkpLaMCyLjJplpoPCXPOHcMl7eJ7pUyrdLijQ6fRt85fsXomWbfgMRg0GIIaIMdhecFx7w8x0Q67OL7lii3j2Z49GaRbDliBe4sVNxmOPbvl6q1x8Gk0pd0Q6QxhruXItkAANwA4Rpz5PZJBaJhpNuY5jYFvizv72163j+r+XiEofm5E+C069JBTNNXA3XNjboORvChe4x0x3Sm9h19YXKlVpql+IJJ7ssj1x3kdtJBKhb22V+jtIPh2JWxBFiDuPyPXI65yg+xYnXGa7nh0lr1tEoMJSZTkWLb+nhmPWalUNrbRUlF+Eyk0JrDVwgKqodCb8017A8dkjdeTidSZ7a2t9XZKUqFOjfiuZHWMgAeu0YeNPuzz6E082ESoi0w3OG+0SRY7JG62e/pgk7qUmmT6E1lfCU+bFJXG1cbTFbX3jIG4vnB2STx1OXIHROsVTD1Kj7Ac1SC1zbMEnIi/TGDlQpJfYm1JP+LX7D+kZD5S/l9jYNoUPihiTV/V7BpbORFiN9+voj6Kauaq4aKqrqWNohMYUpE3NHZJ7traAPVcG0EsxzGl3htnq1rwy0sA1NBZVNIDuqL5xn4Hw5SlkKUvuYESM20PGJEOIzJ4S0PeNol5I2SykeIokEZkiDEFho8OksCapDLvuAezp7pPTfwWmUcvF5yUkWNCmFUKNwFpVnJyezRrr4RSRMsjJAxBYaDWAGgxBCPHj8Fte0vvcV6f5yWu3XZlyi/XZlORLW0/BpRafgaOSoRjEiGMcNFNp7HbI5tTmR7XUOjtPpLuJRyfJgWWaWigE0SBBCINBCCyVBrBJYBQSZDxg0EptujMPSfkMVT8beJjBKEfYcVD8R8TGJFCPsLbPSfExmSRil4QoxMh4IaEI2tkg9j8Jj8Jew5s1lA8YRIIzJEHBDQYghoMQGESLBYaCEEJEiwA0GILCQSwWEeXHaPFX2hk3ke35ySq7j2ZboyHHsykq0ip2WFiOEuRkpLaNSE1NbQBjkyIsRWFNS53KL/yhwjyeh29IxlaqajFjvJuZtwjxjoqt7GEcJBCMHEKMSINYLJqwoJMh4wcR4JIvI4iDQQjMkQhGJIhRg0OBEotvSDTJ6VPpmhTiqPd+R+RLLOvsNtmoWcqePIkEZkiDghoMQQ0GIDCJFgsNBCCEg1gBokEFhIJYLCJFgBICvh1qCzC/qOwxo2Sj4Jq7JQe0ylxeiXp5r7S9XvDtHyl2vIjLz2NOnKjPyZbWavZVp/EST2Dd5zWwoJtyJrZdtIz00yFBCCGghGDiFGJEGsFk1YUEmQ8YOI8EkXkcRBoIRiRCiDRIqXktWPObD2TItppV0xgu3kXLZIIY6Hi2OagTk2eQrwSCCw0HBDQYghoMQGESLBYaCEEJBrADRIILCQaQWEGIDCQUiYYYgsdHMdea+1jHA/QRF77XPrOr6ZHVCb8svV713KGaBMghGJEFGDiFESINYDJYBQSdDxg4jiMSIICFGEpeEEmGKZliGHN92GpEqoJbrx4wH2yQSfwEPGCQQgEiD2D8MYI12PoGnVdDwdh3XNpy9qcZNHkk48ZNEYkTEg4ISDEENBiAwiRYLDQQghINYAaJBBYSDWCwgxACQQkTCJEgtbDRx7TdXbxWIb/UVB3K7KPITtMVapivsi9DweSWCVBQSRBCMGhxGZIiRBJaK1bLiySLJQkufgYe5IpDhBCWHWg4yDVRDjj1x+gabDEPSXgNCEQcQxBDQQjMNDxiSIdNSTYbybDtOQgDyfGLZ0v+6Zjckc//ABNha5YDYqiqPdcZ9TD5zCzq9S5fQ5HJh+baM8JnlYOCEgxBDQYgMIkWCw0EIISDWAGiQQWEg1gsIMQGEgxImEEkZeQ0ca0mLV64/wBTW/5Wna4/yo/oi/HwAi3mrj40Zw5MkQewJN+CgSBCmIzwoBIIIILwoEkQlWFXjwhLkiREgk4aCEZhxHWCyVBCAEIREsQxBCQQjBoeMHE0eoeijicUhI9ikdtu0e6PG3cDI5vSKHVMlV0uP1Z2a0rnInk0jgVr0yjbjuPEHgRIra1ZHiwZwUlo5zj8C9ByjjPg3BhwImFbU65aZmWQcHpkMiGQYghoMQGESLBYaCEEJBrADRIILCQawWEGIDCQUiYSDSC3oNHJtaqHN4yuOmptfjG16kzscCXLHi/sXq/B4aU6LD+UTRJRLQYQjMJDiCSIIQSRBRmGghGYcR1gslQQgBCERJEMQQ0EIxIejBYN67rTpoWdjYAep6AOmC3oGy2NUHKT7HZ9VtBJgaIQWLnN3+JurqErSltnIZeTLIs5Px9C8tBKo8Qjw6S0cmIXZdb9DcQekGRW1RsWmDOCktMx+ktWqtIkoOcXq97vHymVbhzj47opyolF9ioZCMipB68vWUXFryiPiwhI2OSLBYaCEEJBrADRIILCQawWEGIDCQQkQSDEYNHP+UfCbNanU4VKez+8h+RE6Po9u6nD6ot0vsZinOvwvllmJKJaDQQjBIcQSRBCCSIKMw0EIzDiOsFkqCEAIcREkQqa7RsBc9AzPhBbQ/OK7tmk0LqXisUQTTNJP/K+Rt+yu/0EilNFO7qVNXh7Z03VzVulgUsgJcj2qrW2m8Nw6pDKTZz+Tl2XvcvHsXsArCiEKIQohAmMxIotPbpmZXggsMdU949syJeSqOsBhoIQQkGsANEggsJBrBYQYgMJBCRBIMRg0ZLlK/6NH75vyGbfRfikWaDC052+F8stxJRLQaCEYJDiCSIIQSRBRmGghGYcR1gslQQgBB0t4iZJE6hqXvkE/Bj55vBuErmGOIwh4hCiEf/Z',
        status: false
    },
    {
        id: '567-567-567',
        name: 'Sony Liv',
        logoUrl: 'data:image/jpeg;base64,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',
        status: false
    },
    {
        id: '678-678-678',
        name: 'Disney HotStar',
        logoUrl: 'data:image/png;base64,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',
        status: false
    },
    {
        id: '111-111-111',
        name: 'Zee5',
        logoUrl: 'data:image/png;base64,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',
        status: false
    },
    {
        id: '009-009-009',
        name: 'Mubi',
        logoUrl: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAARMAAAC3CAMAAAAGjUrGAAABDlBMVEUZI/3///8fKPb///0ZJPv//v8AAO0YIvwAAOoTH/8AAOYZJPr4+//8//////szN+MAAOJDSOvO0P0AAPENGv3///gAEP/j6fjs7Pzk5P+fpPUAAPbh3/+qrfXOz/Dk4//V2vsAANpKUdbS1v/r6f////GGhujy+/88QuD29P/g5PpaXt27v/RaYdmCh+bq8P02P9WJjuGTmevQ2vRYWOW5uPJSVux7e/J/g/HCw/ggLeSioOiqrOh3e9zDx/EeIuCRlfE3PvA/RPIsM/Vjau5tc+xFTOWhluORpuR+d+V1dta4s/Rgc+WvuvIAGN9+hfAZHNV2fLu1uOJaXM2jnN9hZOzFx+mKhtuEluJMUuxPedlnAAAMaklEQVR4nO2dC1fbRhbHNZ6RRqOxZIFkYSpCcCABO4Apj7SxqaHdpC152GyztN3v/0X23tHD8iPZPT17rOxyfz0ngGzJo7/u3JemsmURBPFXkULUPYSvDqEFqbKAEKTJEiTIMrLuARAEQRAEQRAEQRAEQRAEQRAEQRAEQRAEQRAEQRAEQRAEQRAEQRAEQRAEQRAEQRAE8d9AxFEYdqNYWFrgQxa0ZSVut+ulSgpd9+DqoeG+PO40t45PHJU9dULE3unmoLnzpJXKxqN8DoUKjxnnQcDYcSjMUyfisz5sYQH3T6O6R1cHWoS7LPB9zoCLELeoo0MQ5Dm37YB9+xhF0ck5Y77vo6kwdp7CFueCw4YA//HZkap7hDXQHTADh9nCO13LkpeokY9/M5tdpXUPcP2oV4wVorA2fxNb0XWuCWwL7AO37hGun+Q7Zs4eRYHfvgdNXmeawBbwKL3h45s8RhNe2kqpCQoCBM1HqIk6gpibiwI/LmMrHQWs0IQF/bDuEa4f3T3g5expo4+NNxj3M6cbBPz6EQZjmf5QTByIMtNEWtodMzQTo8nhzWNM7hvuFXvOjYtlV8Yo1KQD8tgoiX8S1z2+WlDOuZ3ZySiSQggt5P048y+Dh0eYnRhU2rrdvbi4HUayYZ5QKZR7+X537/j7bizlI3nu3vzzSiV4DM/x3MRSjQa8BCLohnJcJ02E/mokMc/OFJ8bjRQrnq0py92+eGRZHF4ubZf5wfMNy58uZV1GIy2FSGtZE6mylwSMzfxWeU3AbhJ3g4uuFqm8zewopV44uorjWOGHi4Ut82+Ja8nfpFA/bgA//7h8SRobBi0tad7zU+UtWv3NvNiAQ/y0MU9r4sb5yQpptvw4r4nyhm+mb4aeKp5kKuLw6O3d5U2em+B7o8mrD9/9HMbW+p/sKYV7YXx+J7QWLkr0C4McirOWstx9SB74QVTastDJrdntWWx522we++D1Q5aNCuH0MJ5sepXjqqQ1hnyE++OjRGvsqCjvZAd3bJ6HOju66o46uKX/1rFUDZrsZmnlaVzNloRQNz0bs/AmarKJCdXTOU1emIQcNdkxOWhJgC20vVYsjCZ4akFFE91IH6Deg5ovaPufcHI0hHMN+T5kKO2gf2/epCZ7UCIzG4507ap1Z3HScnaz8gPT7Mp2nf6aVSC5JnDeT51FTYJME9+uasKz+vZMlZpU7USqoya3baj62oF9iB0kkZ5mRSBvg5bYKhDeLmsHKGbbZ7fpup1KqQlnL+ZSSHV/iE0fltsJnCZ/6hVeAaIFaMJ4oQnj1bnDTX+oEyqQbkkT7YxNawCbA7Y9diG83DQZioQHCdgdZPvJO7CbfB4GzZvaNGG8M1eRRtemkcwrmmzPNNGZJu1SE9755knO+z6cUdBm55EwdgJ7jmeaiBvfNBnBLnxQoiVFcooGmTVQeLAHftbZZLN2gv/CWasiVU0Y+yMuIiNE2lYzdxKlJuwzmrjGn2x6kQNEUZR6I4YT48DRWrhGk4qdxB+gqvHzucLZd7FOPwalJjYbNKTqDmaa+O3xuitlIUGTvJnRmYjcTLWI3hdN06omssi0tEBNgoom++4saHpjFviBfSQbuSYVO4mnLJcE5gVn01g44yAo7YQftpS697ldmEnQ3l23naAm2cSFK3OeFoaihuZK2fOauKs0UTNN8mTDvAb+92Ws87mzOWuyxm9L5wMvgJ2I6BjnTm4ZfDCRVrcXzJwTr8VO8KN7ePq9bm4nIv2IY2zacDnLuLOkib9oJ7miAmzBBnd5EpdxZ6aJumfcRHHTU7Lh4PLFTBOb7UFl7Ozziiin624fCPAnOJw/e+hVRtklEcabMPZ3HPpMk6cVTayZJl6pCRS5GK1l9ATv6vln0mgCs6KiieW+Bk+aadIOxjgv7nuY0+SavAMF4jvWzo2XBYfDtedsuSa/YZhhvZss3XauTGr7CYUpczYG+cmXNclLwvge7+vxLfSxS5qA3r0g96Btk59Y7ikrNGG7Lma2bunjODtNGzVpsntmBgGGAqeljmxMTa4/MfZFTYqczWjSdUODF73soCSQ72hrhZ004lelA32G00I0nH9A1mo06U+UKQ0nu7km7Jd1e9hs7uD57nU3TeQbWlrr6DXDk5qc4rBWa2L8CYSKQhNmNwvykxmbjHRZEyjEb15jFdQ8HhbBP3q5j3sN/uzCdIMBaCu8PcDD7J2Yi7RmTURmJzvug7lQ1ymUZC2Teozd88/bCWrCq5qgLwjKKcDYb6amXKGJhY224cOHk2GqitUmOnbPPtw966bYdoMZBwlwGl6+fXsWxY31fy0KZN/GTJ+6bh99fXMCRdoYXYX/DO92f8lOqpqgGAGv3M36fSKVyDRhi5pYDRUnUP7NTldYcZIkCsvkbCv8ZjoqsobvRck14dtu/C7zKI4487FiGXvpN/N2sr0Yd/xKbl+teCAle84OW7FWy7E4/1RhVifNTAAtRjSq3lTW9s05Zu7AqA9cK+ybRKWRvjeZ5mWcfsPnNfE+E3dQk+b+Xs5+B3e3/b4HV9sZLGkitZZJ5DoJxu5iY5wCiaxoIPNptH5MfmI00erEXORptwlRh40jsaAJ1IB61kWt2ompd7peTtjdeGqSslEiUBPM7WeaQAIj0rPR5t7FqBUVE0N5Jx93dsanYSU9E+Y2xzq1KD95pkkDMngIwf0/IJfiHJRY0sQVsz5ggkmFXYnFUVHSCxHf9DAD7nXBnyxogpbgjvKJNnKNKNoaZqGX9U7gKLru75rK8hPGDjxIHC4hIWd2D1sZv8FZgCZZvSM8U7xve5XeaPIH535Vk2LBHmii05GNJV1L6cV6B2ad8xGyVMzmWXDlmHWPk62shQRe+11avyZFbg+awGg3A9Mjg+t/JKuaQD6OVxGy3LJZkD4J4I3+mVrWBAqed6av9CbW7qKPNe2SwEgCOdA0gU14v9g3n2xz/0bUr4mcaSLUsyx7hDQlElVNTFS2odAtdpM63Avg3AaTqibl184lWQy7kyticXjA7UwS7rOD0JLqZ9OaMxpB8pzWrAiWbeXcEdkdfp+ZHmxVEyt+iZKAWyh66BLmGTofrOOzuGM0KeJEOjKaXJre43zOpjayNi82s8HnmPUnzJ+VN/1I1P1dbTqLxZkmUOigIlk2W9FE6fDAdF+njsqGG4dbIEk7uIutRR+rlIrve6ZhNFlhJ8k07wwAoP80tpxx2X3lQXv9ZfASushPjCZWdMzhHJpdNacJ/PotGrbNbkM3jpPUPdqBrJUFHRdmSxmL8xrQDTd2jVfad7VeytmSKceJk4ngs+/nNGFBe9Cq/dswM03sQhModWCWj7LEoaKJFV1g9m7z/vmHV5fTsd/225CxYSctt5Nmv7+93Ue2zd2qIGCf4ixnm++z/bPwWUaTB5g719yfZcEDt3ZNTB5rG00M0TUMCxwnFh5VTdQQ+8Z2eT0hDrdxxa8oNOHFfa+gjc4XzOjYaeilWGypbocXjUZMYaQFrqnUBNfHfgXfhenu4vXqG00gmW712G1q5XbyHGY8agKjjI8G4D/axvAhq4dQ64+y1oa3ExivaZrvCLwLbOgYU/ts7rSrcSf9s1AEtPkW1255e0GWnmBzbahqV8SC/ARvZm6hJiAJWHIP7/NkmsAVZz5kXhBPdBxeQUrnZ54QHMDBSWSsyXK326yKqY47p66pbt0B/lXN2bS7W8ycYNdcCdk6zHZst9k0rd9KpI6OB1tbnT2jiYDSdDjFoGN6iLe9ra3BdguDo25YKj16P8gaqby390OY5Msz3P3O1jwXT96E+YqjaB/+7lxV+2w6/Jh7lCvPxHahzi6yLc0fHEvXHncg1w67GC3wD1N1qbhMDzS+Eppi1ayqiaPJP+/Of/319KHlJrM7lnnACU0BGHrwM4ozE4LyBl/ohpXb4CC48+pqu7d9dVmWSNqb/t4ZQFW4/kbjSvLlNnh15KLZZqt1Zm+FLdKJ0iS2TIdwlZmbRUllfo57wH+L641U5LlRJGerO1TiupE7u5FTs61Ic3qri4wVbZ3s/1azytVeWQM176JmP3SZz6L7WNUZEtrSjblGAO6sS2uqPfSIz3cqcNHVv99fL0lXrkr6XF9IrOo8zwZRuyZf4K+uscs1+ewiwJlkc1vLS/M1a/JXmdPkC6f3mDQhCIIgCIIgCIIgCIIgCIIgCIIgCIIgCIIgCIIgCIIgCIIgCIIgCIIgCIIgCIIgCOJ/CPkf/Ltyl/9PGsQi/wIM4f0yQwzHiAAAAABJRU5ErkJggg==',
        status: false
    },
    {
        id: '123-dfg-126',
        name: 'EROSNOW',
        logoUrl: 'data:image/jpeg;base64,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',
        status: true
    },
    {
        id: '234-234-236',
        name: 'ALT Balaji',
        logoUrl: 'data:image/jpeg;base64,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',
        status: true
    },
    {
        id: '345-345-3gtd6',
        name: 'Voot',
        logoUrl: 'data:image/jpeg;base64,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',
        status: false
    },
    {
        id: '567-567-drg7',
        name: 'Sony Liv',
        logoUrl: 'data:image/jpeg;base64,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',
        status: false
    },
    {
        id: '678-678-678',
        name: 'Disney HotStar',
        logoUrl: 'data:image/png;base64,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',
        status: false
    },
    {
        id: '111-1ry-111',
        name: 'Zee5',
        logoUrl: 'data:image/png;base64,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',
        status: false
    },
    {
        id: '009-ty-009',
        name: 'Mubi',
        logoUrl: 'data:image/png;base64,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',
        status: false
    }
];
class ContentProviderComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this.message = "Please press OK to continue.";
        this.cps = data;
    }
    ngOnInit() {
    }
    openDeleteConfirmModal() {
        const dialogRef = this.dialog.open(CPDeleteConfirmDialog, {
            data: { message: this.message },
            width: '40%'
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
    openEditConfirmModal() {
        const dialogRef = this.dialog.open(_add_content_provider_add_content_provider_component__WEBPACK_IMPORTED_MODULE_1__["AddContentProviderComponent"], {
            width: '60%', disableClose: true,
            data: { message: this.message }
        });
        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }
    openNewCPModal() {
        const dialogRef = this.dialog.open(_add_content_provider_add_content_provider_component__WEBPACK_IMPORTED_MODULE_1__["AddContentProviderComponent"], {
            width: '60%', disableClose: true
        });
    }
}
ContentProviderComponent.ɵfac = function ContentProviderComponent_Factory(t) { return new (t || ContentProviderComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_0__["MatDialog"])); };
ContentProviderComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵdefineComponent"]({ type: ContentProviderComponent, selectors: [["app-content-provider"]], decls: 3, vars: 1, consts: [[1, "cms-container"], ["fxLayout", "row wrap", "fxLayoutGap", "32px", "fxLayoutAlign", "flex-start"], ["fxFlex", "0 1 calc(25% - 32px)", "class", "cms-card", 4, "ngFor", "ngForOf"], ["fxFlex", "0 1 calc(25% - 32px)", 1, "cms-card"], [4, "ngIf"], ["mat-card-avatar", ""], ["mat-list-icon", "", "class", "cp-status-active", 4, "ngIf"], ["mat-list-icon", "", "class", "cp-status-inactive", 4, "ngIf"], ["mat-card-image", "", "alt", "Logo of content provider", 1, "cms-cp-logo", 3, "src"], ["mat-button", "", 3, "click"], ["mat-list-icon", "", 1, "cp-status-active"], ["mat-list-icon", "", 1, "cp-status-inactive"], ["mat-card-image", "", "src", "https://cdn.iconscout.com/icon/free/png-512/add-new-1439785-1214356.png", 1, "cms-cp-logo", 3, "click"]], template: function ContentProviderComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](0, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](1, "div", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtemplate"](2, ContentProviderComponent_mat_card_2_Template, 3, 2, "mat-card", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵadvance"](2);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵproperty"]("ngForOf", ctx.cps);
    } }, directives: [_angular_flex_layout_flex__WEBPACK_IMPORTED_MODULE_3__["DefaultLayoutDirective"], _angular_flex_layout_flex__WEBPACK_IMPORTED_MODULE_3__["DefaultLayoutGapDirective"], _angular_flex_layout_flex__WEBPACK_IMPORTED_MODULE_3__["DefaultLayoutAlignDirective"], _angular_common__WEBPACK_IMPORTED_MODULE_4__["NgForOf"], _angular_material_card__WEBPACK_IMPORTED_MODULE_5__["MatCard"], _angular_flex_layout_flex__WEBPACK_IMPORTED_MODULE_3__["DefaultFlexDirective"], _angular_common__WEBPACK_IMPORTED_MODULE_4__["NgIf"], _angular_material_card__WEBPACK_IMPORTED_MODULE_5__["MatCardHeader"], _angular_material_card__WEBPACK_IMPORTED_MODULE_5__["MatCardAvatar"], _angular_material_card__WEBPACK_IMPORTED_MODULE_5__["MatCardTitle"], _angular_material_card__WEBPACK_IMPORTED_MODULE_5__["MatCardImage"], _angular_material_card__WEBPACK_IMPORTED_MODULE_5__["MatCardContent"], _angular_material_card__WEBPACK_IMPORTED_MODULE_5__["MatCardActions"], _angular_material_button__WEBPACK_IMPORTED_MODULE_6__["MatButton"], _angular_material_icon__WEBPACK_IMPORTED_MODULE_7__["MatIcon"], _angular_material_list__WEBPACK_IMPORTED_MODULE_8__["MatListIconCssMatStyler"]], styles: [_c0] });
class CPEditConfirmDialog {
    constructor(dialogRef, data) {
        this.dialogRef = dialogRef;
        this.data = data;
    }
    onCancelUpload() {
        this.dialogRef.close();
    }
    onConfirmUpload() {
        this.dialogRef.close();
    }
}
CPEditConfirmDialog.ɵfac = function CPEditConfirmDialog_Factory(t) { return new (t || CPEditConfirmDialog)(_angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_0__["MatDialogRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_0__["MAT_DIALOG_DATA"])); };
CPEditConfirmDialog.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵdefineComponent"]({ type: CPEditConfirmDialog, selectors: [["cp-edit-confirm-dialog"]], decls: 10, vars: 1, consts: [["mat-dialog-title", ""], ["mat-dialog-content", ""], ["mat-dialog-actions", ""], ["mat-button", "", "mat-dialog-close", "", 3, "click"], ["mat-button", "", 3, "click"]], template: function CPEditConfirmDialog_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](0, "h2", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](1, "Confirm");
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](2, "div", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](3, "p");
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](5, "div", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](6, "button", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵlistener"]("click", function CPEditConfirmDialog_Template_button_click_6_listener() { return ctx.onCancelUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](7, "Cancel");
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](8, "button", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵlistener"]("click", function CPEditConfirmDialog_Template_button_click_8_listener() { return ctx.onConfirmUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](9, "Save");
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtextInterpolate"](ctx.data.message);
    } }, styles: [_c0] });
class CPDeleteConfirmDialog {
    constructor(dialogRef, data) {
        this.dialogRef = dialogRef;
        this.data = data;
    }
    onCancelUpload() {
        this.dialogRef.close();
    }
    onConfirmUpload() {
        this.dialogRef.close();
    }
}
CPDeleteConfirmDialog.ɵfac = function CPDeleteConfirmDialog_Factory(t) { return new (t || CPDeleteConfirmDialog)(_angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_0__["MatDialogRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵdirectiveInject"](_angular_material_dialog__WEBPACK_IMPORTED_MODULE_0__["MAT_DIALOG_DATA"])); };
CPDeleteConfirmDialog.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵdefineComponent"]({ type: CPDeleteConfirmDialog, selectors: [["cp-delete-confirm-dialog"]], decls: 10, vars: 1, consts: [["mat-dialog-title", ""], ["mat-dialog-content", ""], ["mat-dialog-actions", "", 1, "btn-sec"], ["mat-raised-button", "", "mat-dialog-close", "", 1, "discard-btn", 3, "click"], ["mat-raised-button", "", "color", "primary", 1, "update-btn", 3, "click"]], template: function CPDeleteConfirmDialog_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](0, "h2", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](1, "Confirm");
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](2, "div", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](3, "p");
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](5, "div", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](6, "button", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵlistener"]("click", function CPDeleteConfirmDialog_Template_button_click_6_listener() { return ctx.onCancelUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](7, "Cancel");
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementStart"](8, "button", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵlistener"]("click", function CPDeleteConfirmDialog_Template_button_click_8_listener() { return ctx.onConfirmUpload(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtext"](9, "OK");
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_2__["ɵɵtextInterpolate"](ctx.data.message);
    } }, styles: [_c0] });


/***/ }),

/***/ "vY5A":
/*!***************************************!*\
  !*** ./src/app/app-routing.module.ts ***!
  \***************************************/
/*! exports provided: AppRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppRoutingModule", function() { return AppRoutingModule; });
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/router */ "tyNb");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "fXoL");



const routes = [];
class AppRoutingModule {
}
AppRoutingModule.ɵmod = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineNgModule"]({ type: AppRoutingModule });
AppRoutingModule.ɵinj = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineInjector"]({ factory: function AppRoutingModule_Factory(t) { return new (t || AppRoutingModule)(); }, imports: [[_angular_router__WEBPACK_IMPORTED_MODULE_0__["RouterModule"].forRoot(routes, { relativeLinkResolution: 'legacy' })], _angular_router__WEBPACK_IMPORTED_MODULE_0__["RouterModule"]] });
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵsetNgModuleScope"](AppRoutingModule, { imports: [_angular_router__WEBPACK_IMPORTED_MODULE_0__["RouterModule"]], exports: [_angular_router__WEBPACK_IMPORTED_MODULE_0__["RouterModule"]] }); })();


/***/ }),

/***/ "zUnb":
/*!*********************!*\
  !*** ./src/main.ts ***!
  \*********************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/platform-browser */ "jhN1");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _app_app_module__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./app/app.module */ "ZAI4");
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./environments/environment */ "AytR");




if (_environments_environment__WEBPACK_IMPORTED_MODULE_3__["environment"].production) {
    Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["enableProdMode"])();
}
_angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__["platformBrowser"]().bootstrapModule(_app_app_module__WEBPACK_IMPORTED_MODULE_2__["AppModule"])
    .catch(err => console.error(err));


/***/ }),

/***/ "zn8P":
/*!******************************************************!*\
  !*** ./$$_lazy_route_resource lazy namespace object ***!
  \******************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

function webpackEmptyAsyncContext(req) {
	// Here Promise.resolve().then() is used instead of new Promise() to prevent
	// uncaught exception popping up in devtools
	return Promise.resolve().then(function() {
		var e = new Error("Cannot find module '" + req + "'");
		e.code = 'MODULE_NOT_FOUND';
		throw e;
	});
}
webpackEmptyAsyncContext.keys = function() { return []; };
webpackEmptyAsyncContext.resolve = webpackEmptyAsyncContext;
module.exports = webpackEmptyAsyncContext;
webpackEmptyAsyncContext.id = "zn8P";

/***/ })

},[[0,"runtime","vendor"]]]);
//# sourceMappingURL=main-es2015.js.map