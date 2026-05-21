import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { forkJoin, Observable, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { MaterialModule } from '../../../../shared/material.module';
import { AgendaMedicaService } from '../../../../core/services/agenda-medica.service';
import { AuthService } from '../../../../core/services/auth.service';
import { CatalogoService } from '../../../../core/services/catalogo.service';
import { CitaService } from '../../../../core/services/cita.service';
import { HistorialCitaService } from '../../../../core/services/historial-cita.service';
import { UserScopeService } from '../../../../core/services/user-scope.service';
import { agendaMedicaItemsDto } from '../../../../core/models/agenda-medica.model';
import { catalogoItem } from '../../../../core/models/catalogo.model';
import { citaItemsDto } from '../../../../core/models/cita.model';
@Component({
  selector: 'app-cita-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './cita-form.component.html',
})
export class CitaFormComponent implements OnInit {
  esEdicion = false;
  loading = false;
  pacientes: catalogoItem[] = [];
  medicos: catalogoItem[] = [];
  estadosCita: catalogoItem[] = [];
  modalidades: catalogoItem[] = [];
  estadosVigencia: catalogoItem[] = [];
  agendasDisponibles: agendaMedicaItemsDto[] = [];
  fechasAgendaDisponibles: string[] = [];
  currentPacienteId: number | null = null;
  cargandoAgenda = false;
  form: FormGroup;
  readonly estadoProgramadaId = 1;
  readonly estadoCanceladaId = 4;
  readonly estadoActivoId = 1;
  constructor(
    private fb: FormBuilder,
    private svc: CitaService,
    private agendaService: AgendaMedicaService,
    private auth: AuthService,
    private cat: CatalogoService,
    private historialService: HistorialCitaService,
    private snack: MatSnackBar,
    private userScope: UserScopeService,
    public ref: MatDialogRef<CitaFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    this.form = this.fb.group({
      idPaciente: [null as any, [Validators.required]],
      idMedico: [null as any, [Validators.required]],
      idAgendaMedica: [null as any, [Validators.required]],
      idEstadoCita: [null as any, [Validators.required]],
      idModalidadCita: [null as any, [Validators.required]],
      idTblEstadoVigencia: [null as any, [Validators.required]],
      fechaCita: [null as any, [Validators.required]],
      horaCita: [null as any, [Validators.required]],
      motivo: [null as any, [Validators.required]],
      observacion: [null as any],
    });
  }
  get isAdmin(): boolean {
    return this.auth.isAdmin();
  }
  ngOnInit(): void {
    if (this.isAdmin) this.cat.getPacientes().subscribe((r) => (this.pacientes = r));
    this.cat.getMedicos().subscribe((r) => (this.medicos = r));
    this.cat.getEstadosCita().subscribe((r) => (this.estadosCita = r));
    this.cat.getModalidadesCita().subscribe((r) => (this.modalidades = r));
    this.cat.getEstadosVigencia().subscribe((r) => (this.estadosVigencia = r));
    this.aplicarValoresIniciales();
    if (this.data) {
      this.esEdicion = true;
      this.form.patchValue(this.data, { emitEvent: false });
    }
    this.configurarPermisos();
    this.configurarEscuchaAgenda();
    if (this.form.get('idMedico')?.value) this.recargarAgendaDisponible();
  }
  aplicarValoresIniciales(): void {
    if (!this.data) {
      this.form.patchValue(
        {
          idEstadoCita: this.estadoProgramadaId,
          idTblEstadoVigencia: this.estadoActivoId,
        },
        { emitEvent: false },
      );
    }
  }
  configurarPermisos(): void {
    if (!this.esEdicion) {
      this.form.get('idEstadoCita')?.disable({ emitEvent: false });
      this.form.get('idTblEstadoVigencia')?.disable({ emitEvent: false });
    }
    if (!this.isAdmin) {
      this.form.get('idPaciente')?.disable({ emitEvent: false });
      this.form.get('idEstadoCita')?.disable({ emitEvent: false });
      this.form.get('idTblEstadoVigencia')?.disable({ emitEvent: false });
      if (this.esEdicion) this.form.disable({ emitEvent: false });
      this.userScope.getCurrentPaciente().subscribe((paciente) => {
        this.currentPacienteId = paciente?.idPaciente ?? null;
        if (!this.currentPacienteId) {
          this.snack.open(
            'No se encontro un paciente vinculado al usuario autenticado.',
            'Cerrar',
            { duration: 5000 },
          );
          this.form.disable({ emitEvent: false });
          return;
        }
        this.form.patchValue({ idPaciente: this.currentPacienteId }, { emitEvent: false });
      });
      return;
    }
    if (this.esEdicion) {
      this.form.get('idEstadoCita')?.enable({ emitEvent: false });
      this.form.get('idTblEstadoVigencia')?.enable({ emitEvent: false });
    }
  }
  configurarEscuchaAgenda(): void {
    this.form.get('idMedico')?.valueChanges.subscribe(() => {
      this.limpiarAgendaSeleccionada();
      this.recargarAgendaDisponible();
    });
    this.form.get('fechaCita')?.valueChanges.subscribe(() => {
      this.limpiarAgendaSeleccionada();
      this.recargarAgendaDisponible();
    });
  }
  limpiarAgendaSeleccionada(): void {
    this.form.patchValue({ idAgendaMedica: null, horaCita: null }, { emitEvent: false });
  }
  recargarAgendaDisponible(): void {
    const idMedico = Number(this.form.get('idMedico')?.value ?? 0);
    if (!idMedico) {
      this.agendasDisponibles = [];
      this.fechasAgendaDisponibles = [];
      return;
    }
    this.cargandoAgenda = true;
    const fecha = this.form.get('fechaCita')?.value ?? '';
    forkJoin({
      agendas: this.agendaService.listar({
        pageSize: 500,
        skip: 0,
        sortField: 'Id',
        sortDir: 'asc',
        filter: { idMedico: String(idMedico), fecha } as any,
      }),
      citas: this.svc.listar({
        pageSize: 500,
        skip: 0,
        sortField: 'Id',
        sortDir: 'asc',
        filter: { idMedico: String(idMedico) } as any,
      }),
    }).subscribe({
      next: ({ agendas, citas }) => {
        const agendaActual = Number(this.data?.idAgendaMedica ?? 0);
        const ocupadas = new Set(
          citas.data
            .filter((item) => this.bloqueaAgenda(item))
            .filter((item) => item.guid !== this.data?.guid)
            .map((item) => item.idAgendaMedica),
        );
        this.agendasDisponibles = agendas.data.filter(
          (item) =>
            item.idTblEstadoVigencia === this.estadoActivoId &&
            item.esDisponible &&
            (!ocupadas.has(item.idAgendaMedica) || item.idAgendaMedica === agendaActual),
        );
        this.fechasAgendaDisponibles = [...new Set(this.agendasDisponibles.map((item) => item.fecha))];
        if (agendaActual) {
          const agendaSeleccionada = this.agendasDisponibles.find(
            (item) => item.idAgendaMedica === agendaActual,
          );
          if (agendaSeleccionada) this.seleccionarAgenda(agendaSeleccionada.idAgendaMedica, false);
        }
        this.cargandoAgenda = false;
      },
      error: () => {
        this.cargandoAgenda = false;
      },
    });
  }
  bloqueaAgenda(item: citaItemsDto): boolean {
    return item.idTblEstadoVigencia === this.estadoActivoId && item.idEstadoCita !== this.estadoCanceladaId;
  }
  seleccionarAgenda(idAgendaMedica: number, syncDate = true): void {
    const agenda = this.agendasDisponibles.find((item) => item.idAgendaMedica === Number(idAgendaMedica));
    if (!agenda) return;
    this.form.patchValue(
      {
        idAgendaMedica: agenda.idAgendaMedica,
        horaCita: agenda.horaInicio,
        fechaCita: syncDate ? agenda.fecha : this.form.get('fechaCita')?.value ?? agenda.fecha,
      },
      { emitEvent: false },
    );
  }
  agendaSeleccionadaTexto(): string {
    const agenda = this.agendasDisponibles.find(
      (item) => item.idAgendaMedica === Number(this.form.get('idAgendaMedica')?.value),
    );
    return agenda ? `${agenda.fecha} | ${agenda.horaInicio} - ${agenda.horaFin}` : 'Sin horario seleccionado';
  }
  estadoTexto(idEstadoCita: number): string {
    return this.estadosCita.find((item) => item.value === idEstadoCita)?.text ?? `Estado ${idEstadoCita}`;
  }
  guardar(): void {
    if (!this.isAdmin && this.esEdicion) return;
    this.form.markAllAsTouched();
    if (this.form.invalid) return;
    const body = this.form.getRawValue();
    if (!this.agendasDisponibles.some((item) => item.idAgendaMedica === Number(body.idAgendaMedica))) {
      this.snack.open('Seleccione un horario de agenda disponible.', 'Cerrar', { duration: 4000 });
      return;
    }
    this.loading = true;
    const op = this.esEdicion
      ? this.svc.editar(this.data.guid ?? '', { ...body, guidCita: this.data.guid } as any)
      : this.svc.crear(body as any);
    op.pipe(switchMap((saved) => this.registrarHistorial(saved, body))).subscribe({
      next: () => {
        this.loading = false;
        this.ref.close(true);
      },
      error: () => {
        this.loading = false;
      },
    });
  }
  registrarHistorial(saved: citaItemsDto, body: any): Observable<citaItemsDto> {
    const descripcion = this.buildHistorialDescripcion(body);
    if (!descripcion) return of(saved);
    return this.historialService
      .crear({
        idCita: saved.idCita,
        idEstadoCita: Number(body.idEstadoCita),
        fechaEvento: new Date().toISOString().slice(0, 10),
        idTblEstadoVigencia: this.estadoActivoId,
        descripcion,
      } as any)
      .pipe(
        map(() => saved),
        catchError(() => of(saved)),
      );
  }
  buildHistorialDescripcion(body: any): string | null {
    if (!this.esEdicion) {
      return `Cita registrada para ${body.fechaCita} a las ${body.horaCita}.`;
    }
    if (Number(body.idEstadoCita) !== Number(this.data?.idEstadoCita)) {
      return `Estado de cita actualizado a ${this.estadoTexto(Number(body.idEstadoCita))}.`;
    }
    if (
      Number(body.idAgendaMedica) !== Number(this.data?.idAgendaMedica) ||
      body.fechaCita !== this.data?.fechaCita ||
      body.horaCita !== this.data?.horaCita
    ) {
      return `Cita reprogramada para ${body.fechaCita} a las ${body.horaCita}.`;
    }
    if (body.motivo !== this.data?.motivo || body.observacion !== this.data?.observacion) {
      return 'Informacion complementaria de la cita actualizada.';
    }
    return null;
  }
}
