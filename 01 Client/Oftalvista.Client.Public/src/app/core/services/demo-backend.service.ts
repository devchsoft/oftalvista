import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Observable, delay, of, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { loginRequest, loginResponse } from '../models/auth.model';
import { catalogoItem } from '../models/catalogo.model';
import { paginatedItemsResponse } from '../models/pagination.model';
import { agendaMedicaItemsDto } from '../models/agenda-medica.model';
import { citaItemsDto } from '../models/cita.model';
import { especialidadMedicaItemsDto } from '../models/especialidad-medica.model';
import { historialCitaItemsDto } from '../models/historial-cita.model';
import { medicoItemsDto } from '../models/medico.model';
import { pacienteItemsDto } from '../models/paciente.model';
import { pagoCitaItemsDto } from '../models/pago-cita.model';
import { recordatorioCitaItemsDto } from '../models/recordatorio-cita.model';
import { usuarioItemsDto } from '../models/usuario.model';

interface demoDb {
  usuarios: usuarioItemsDto[];
  especialidades: especialidadMedicaItemsDto[];
  medicos: medicoItemsDto[];
  pacientes: pacienteItemsDto[];
  agendasMedicas: agendaMedicaItemsDto[];
  citas: citaItemsDto[];
  pagosCita: pagoCitaItemsDto[];
  historialCitas: historialCitaItemsDto[];
  recordatoriosCita: recordatorioCitaItemsDto[];
}

@Injectable({ providedIn: 'root' })
export class DemoBackendService {
  private readonly dbKey = 'ov_demo_db';
  private readonly latencyMs = 250;

  handle(req: HttpRequest<unknown>): Observable<HttpEvent<unknown>> {
    try {
      const body = this.route(req);
      return of(new HttpResponse({ status: 200, body })).pipe(delay(this.latencyMs));
    } catch (error) {
      const httpError =
        error instanceof HttpErrorResponse
          ? error
          : new HttpErrorResponse({
              status: 500,
              error: { message: 'Error simulando backend demo.' },
            });
      return throwError(() => httpError);
    }
  }

  private route(req: HttpRequest<unknown>): unknown {
    const path = req.url.replace(environment.apiUrl, '');
    const segments = path.split('/').filter(Boolean);
    const [resource, action, nested] = segments;

    switch (resource) {
      case 'auth':
        return this.handleAuth(req, action);
      case 'catalogo':
        return this.handleCatalogo(action, nested);
      case 'usuario':
        return this.handleEntity<usuarioItemsDto>(
          req,
          'usuarios',
          'idUsuario',
          (item) =>
            this.matchesOption(item.idTipoUsuario, req.params.get('idTipoUsuario')) &&
            this.matchesOption(item.idTipoDocumento, req.params.get('idTipoDocumento')) &&
            this.contains(item.numeroDocumento, req.params.get('numeroDocumento')),
        );
      case 'especialidad-medica':
        return this.handleEntity<especialidadMedicaItemsDto>(
          req,
          'especialidades',
          'idEspecialidadMedica',
          (item) => this.contains(item.codigo, req.params.get('codigo')),
        );
      case 'medico':
        return this.handleEntity<medicoItemsDto>(
          req,
          'medicos',
          'idMedico',
          (item) =>
            this.matchesOption(item.idUsuario, req.params.get('idUsuario')) &&
            this.matchesOption(
              item.idEspecialidadMedica,
              req.params.get('idEspecialidadMedica'),
            ) &&
            this.contains(item.cmp, req.params.get('cmp')),
        );
      case 'paciente':
        return this.handleEntity<pacienteItemsDto>(
          req,
          'pacientes',
          'idPaciente',
          (item) => this.matchesOption(item.idUsuario, req.params.get('idUsuario')),
        );
      case 'agenda-medica':
        return this.handleEntity<agendaMedicaItemsDto>(
          req,
          'agendasMedicas',
          'idAgendaMedica',
          (item) =>
            this.matchesOption(item.idMedico, req.params.get('idMedico')) &&
            this.matchesText(item.fecha, req.params.get('fecha')),
        );
      case 'cita':
        return this.handleEntity<citaItemsDto>(
          req,
          'citas',
          'idCita',
          (item) =>
            this.matchesOption(item.idPaciente, req.params.get('idPaciente')) &&
            this.matchesOption(item.idMedico, req.params.get('idMedico')),
        );
      case 'pago-cita':
        return this.handleEntity<pagoCitaItemsDto>(
          req,
          'pagosCita',
          'idPagoCita',
          (item) => this.matchesOption(item.idCita, req.params.get('idCita')),
        );
      case 'historial-cita':
        return this.handleEntity<historialCitaItemsDto>(
          req,
          'historialCitas',
          'idHistorialCita',
          (item) =>
            this.matchesOption(item.idCita, req.params.get('idCita')) &&
            this.matchesOption(item.idEstadoCita, req.params.get('idEstadoCita')),
        );
      case 'recordatorio-cita':
        return this.handleEntity<recordatorioCitaItemsDto>(
          req,
          'recordatoriosCita',
          'idRecordatorioCita',
          (item) => this.matchesOption(item.idCita, req.params.get('idCita')),
        );
      default:
        throw this.httpError(404, 'Ruta demo no implementada.');
    }
  }

  private handleAuth(req: HttpRequest<unknown>, action?: string): loginResponse {
    if (req.method !== 'POST' || action !== 'login') {
      throw this.httpError(404, 'Ruta de autenticacion demo no implementada.');
    }

    const body = (req.body ?? {}) as loginRequest;
    const db = this.readDb();
    const usuario = db.usuarios.find(
      (item) => item.correo.toLowerCase() === (body.correo ?? '').trim().toLowerCase(),
    );

    if (!usuario || body.claveHash !== 'demo123') {
      throw this.httpError(401, 'Credenciales demo invalidas. Use clave demo123.');
    }

    return {
      token: `demo-token-${usuario.idUsuario}`,
      refreshToken: `demo-refresh-${usuario.idUsuario}`,
      idUsuario: usuario.idUsuario,
      nombres: usuario.nombres,
      apellidos: usuario.apellidos,
      idTipoUsuario: usuario.idTipoUsuario,
      tipoUsuario: this.tipoUsuarioLabel(usuario.idTipoUsuario),
    };
  }

  private handleCatalogo(action?: string, nested?: string): catalogoItem[] {
    const db = this.readDb();

    switch (action) {
      case 'tipos-usuario':
        return [
          { value: 1, text: 'Administrador' },
          { value: 2, text: 'Paciente' },
          { value: 3, text: 'Medico' },
        ];
      case 'tipos-documento':
        return [
          { value: 1, text: 'DNI' },
          { value: 2, text: 'CE' },
          { value: 3, text: 'Pasaporte' },
        ];
      case 'estados-vigencia':
        return [
          { value: 1, text: 'Activo' },
          { value: 2, text: 'Inactivo' },
        ];
      case 'estados-cita':
        return [
          { value: 1, text: 'Programada' },
          { value: 2, text: 'Confirmada' },
          { value: 3, text: 'Atendida' },
          { value: 4, text: 'Cancelada' },
        ];
      case 'modalidades-cita':
        return [
          { value: 1, text: 'Presencial' },
          { value: 2, text: 'Virtual' },
        ];
      case 'metodos-pago':
        return [
          { value: 1, text: 'Efectivo' },
          { value: 2, text: 'Tarjeta' },
          { value: 3, text: 'Transferencia' },
        ];
      case 'estados-pago':
        return [
          { value: 1, text: 'Pendiente' },
          { value: 2, text: 'Pagado' },
          { value: 3, text: 'Anulado' },
        ];
      case 'estados-recordatorio':
        return [
          { value: 1, text: 'Pendiente' },
          { value: 2, text: 'Enviado' },
          { value: 3, text: 'Fallido' },
        ];
      case 'especialidades':
        return db.especialidades.map((item) => ({ value: item.idEspecialidadMedica, text: item.nombre }));
      case 'medicos':
        return db.medicos.map((item) => ({
          value: item.idMedico,
          text: this.usuarioNombre(db.usuarios, item.idUsuario),
        }));
      case 'pacientes':
        return db.pacientes.map((item) => ({
          value: item.idPaciente,
          text: this.usuarioNombre(db.usuarios, item.idUsuario),
        }));
      case 'usuarios':
        return db.usuarios.map((item) => ({
          value: item.idUsuario,
          text: `${item.nombres} ${item.apellidos}`.trim(),
        }));
      case 'agendas-medico':
        return db.agendasMedicas
          .filter((item) => item.idMedico === Number(nested))
          .map((item) => ({
            value: item.idAgendaMedica,
            text: `${item.fecha} ${item.horaInicio} - ${item.horaFin}`,
          }));
      case 'citas':
        return db.citas.map((item) => ({
          value: item.idCita,
          text: `Cita ${item.idCita} - ${item.fechaCita} ${item.horaCita}`,
        }));
      default:
        throw this.httpError(404, 'Catalogo demo no implementado.');
    }
  }

  private handleEntity<T extends { guid?: string }>(
    req: HttpRequest<unknown>,
    key: keyof demoDb,
    idField: keyof T & string,
    matches: (item: T) => boolean,
  ): unknown {
    switch (req.method) {
      case 'GET':
        return this.handleGet(req, key, idField, matches);
      case 'POST':
        return this.handleCreate(req, key, idField);
      case 'PUT':
        return this.handleUpdate(req, key);
      case 'DELETE':
        return this.handleDelete(req, key);
      default:
        throw this.httpError(405, 'Metodo demo no permitido.');
    }
  }

  private handleGet<T extends { guid?: string }>(
    req: HttpRequest<unknown>,
    key: keyof demoDb,
    idField: keyof T & string,
    matches: (item: T) => boolean,
  ): unknown {
    const guid = this.lastSegment(req.url);
    const items = this.readEntity<T>(key);

    if (guid) {
      const item = items.find((current) => current.guid === guid);
      if (!item) throw this.httpError(404, 'Registro demo no encontrado.');
      return this.clone(item);
    }

    const filtered = items.filter(matches);
    return this.paginate(filtered, req, idField);
  }

  private handleCreate<T extends { guid?: string }>(
    req: HttpRequest<unknown>,
    key: keyof demoDb,
    idField: keyof T & string,
  ): T {
    return this.mutateDb((db) => {
      const items = db[key] as unknown as T[];
      const nextId = this.nextId(items, idField);
      const body = this.cleanBody(req.body as Record<string, unknown>);
      const item = {
        ...(body as object),
        [idField]: nextId,
        guid: this.newGuid(String(key)),
      } as T;
      items.unshift(item);
      return this.clone(item);
    });
  }

  private handleUpdate<T extends { guid?: string }>(req: HttpRequest<unknown>, key: keyof demoDb): T {
    const guid = this.lastSegment(req.url);
    if (!guid) throw this.httpError(400, 'Guid demo requerido.');

    return this.mutateDb((db) => {
      const items = db[key] as unknown as T[];
      const index = items.findIndex((item) => item.guid === guid);
      if (index < 0) throw this.httpError(404, 'Registro demo no encontrado.');
      items[index] = {
        ...items[index],
        ...(this.cleanBody(req.body as Record<string, unknown>) as object),
        guid,
      } as T;
      return this.clone(items[index]);
    });
  }

  private handleDelete<T extends { guid?: string }>(req: HttpRequest<unknown>, key: keyof demoDb): null {
    const guid = this.lastSegment(req.url);
    if (!guid) throw this.httpError(400, 'Guid demo requerido.');

    this.mutateDb((db) => {
      const items = db[key] as unknown as T[];
      const index = items.findIndex((item) => item.guid === guid);
      if (index < 0) throw this.httpError(404, 'Registro demo no encontrado.');
      items.splice(index, 1);
      return null;
    });

    return null;
  }

  private paginate<T extends Record<string, unknown>>(
    items: T[],
    req: HttpRequest<unknown>,
    sortField: string,
  ): paginatedItemsResponse<T> {
    const pageSize = Number(req.params.get('pageSize') ?? 10);
    const skip = Number(req.params.get('skip') ?? 0);
    const sortBy = req.params.get('sortField') || sortField;
    const sortDir = (req.params.get('sortDir') || 'asc').toLowerCase();
    const sorted = [...items].sort((a, b) => this.compareValues(a[sortBy], b[sortBy], sortDir));
    const count = sorted.length;
    const data = sorted.slice(skip, skip + pageSize).map((item, index) => ({
      ...item,
      rowNum: skip + index + 1,
      rowCount: count,
    }));

    return {
      pageIndex: Math.floor(skip / pageSize),
      pageSize,
      count,
      data,
    };
  }

  private compareValues(a: unknown, b: unknown, sortDir: string): number {
    if (a == null && b == null) return 0;
    if (a == null) return sortDir === 'desc' ? 1 : -1;
    if (b == null) return sortDir === 'desc' ? -1 : 1;

    const left = String(a).toLowerCase();
    const right = String(b).toLowerCase();
    const result = left < right ? -1 : left > right ? 1 : 0;
    return sortDir === 'desc' ? result * -1 : result;
  }

  private readEntity<T>(key: keyof demoDb): T[] {
    return this.clone(this.readDb()[key] as T[]);
  }

  private readDb(): demoDb {
    const raw = localStorage.getItem(this.dbKey);
    if (!raw) {
      const seeded = this.seedDb();
      this.writeDb(seeded);
      return seeded;
    }

    try {
      return JSON.parse(raw) as demoDb;
    } catch {
      const seeded = this.seedDb();
      this.writeDb(seeded);
      return seeded;
    }
  }

  private writeDb(db: demoDb): void {
    localStorage.setItem(this.dbKey, JSON.stringify(db));
  }

  private mutateDb<T>(callback: (db: demoDb) => T): T {
    const db = this.readDb();
    const result = callback(db);
    this.writeDb(db);
    return result;
  }

  private clone<T>(value: T): T {
    return JSON.parse(JSON.stringify(value)) as T;
  }

  private contains(value: string | undefined, search: string | null): boolean {
    if (!search) return true;
    return (value ?? '').toLowerCase().includes(search.toLowerCase());
  }

  private matchesText(value: string | undefined, search: string | null): boolean {
    if (!search) return true;
    return (value ?? '') === search;
  }

  private matchesOption(value: number | undefined, search: string | null): boolean {
    if (!search) return true;
    return String(value ?? '') === search;
  }

  private cleanBody(body: Record<string, unknown>): Record<string, unknown> {
    return Object.fromEntries(
      Object.entries(body ?? {}).filter(([key]) => key === 'guid' || !key.startsWith('guid')),
    );
  }

  private nextId<T>(items: T[], idField: keyof T & string): number {
    if (!items.length) return 1;
    return Math.max(...items.map((item) => Number((item as Record<string, unknown>)[idField] ?? 0))) + 1;
  }

  private lastSegment(url: string): string | undefined {
    const path = url.replace(environment.apiUrl, '');
    const parts = path.split('/').filter(Boolean);
    return parts.length > 1 ? parts[parts.length - 1] : undefined;
  }

  private newGuid(scope: string): string {
    return `demo-${scope}-${Date.now()}-${Math.random().toString(36).slice(2, 10)}`;
  }

  private usuarioNombre(usuarios: usuarioItemsDto[], idUsuario: number): string {
    const usuario = usuarios.find((item) => item.idUsuario === idUsuario);
    return usuario ? `${usuario.nombres} ${usuario.apellidos}`.trim() : `Usuario ${idUsuario}`;
  }

  private tipoUsuarioLabel(idTipoUsuario: number): string {
    switch (idTipoUsuario) {
      case 1:
        return 'Administrador';
      case 2:
        return 'Paciente';
      case 3:
        return 'Medico';
      default:
        return 'Usuario';
    }
  }

  private httpError(status: number, message: string): HttpErrorResponse {
    return new HttpErrorResponse({ status, error: { message } });
  }

  private seedDb(): demoDb {
    return {
      usuarios: [
        {
          idUsuario: 1,
          guid: 'demo-usuario-1',
          idTipoUsuario: 1,
          idTipoDocumento: 1,
          numeroDocumento: '70000001',
          nombres: 'Admin',
          apellidos: 'Demo',
          correo: 'admin@demo.pe',
          telefono: '999111222',
          idTblEstadoVigencia: 1,
        },
        {
          idUsuario: 2,
          guid: 'demo-usuario-2',
          idTipoUsuario: 3,
          idTipoDocumento: 1,
          numeroDocumento: '70000002',
          nombres: 'Lucia',
          apellidos: 'Salazar',
          correo: 'medico@demo.pe',
          telefono: '999222333',
          idTblEstadoVigencia: 1,
        },
        {
          idUsuario: 3,
          guid: 'demo-usuario-3',
          idTipoUsuario: 2,
          idTipoDocumento: 1,
          numeroDocumento: '70000003',
          nombres: 'Carlos',
          apellidos: 'Vargas',
          correo: 'paciente@demo.pe',
          telefono: '999333444',
          idTblEstadoVigencia: 1,
        },
        {
          idUsuario: 4,
          guid: 'demo-usuario-4',
          idTipoUsuario: 2,
          idTipoDocumento: 1,
          numeroDocumento: '70000004',
          nombres: 'Maria',
          apellidos: 'Paredes',
          correo: 'paciente2@demo.pe',
          telefono: '999444555',
          idTblEstadoVigencia: 1,
        },
      ],
      especialidades: [
        {
          idEspecialidadMedica: 1,
          guid: 'demo-especialidad-1',
          codigo: 'OFT-GEN',
          nombre: 'Oftalmologia General',
          descripcion: 'Consulta integral de oftalmologia.',
          idTblEstadoVigencia: 1,
        },
        {
          idEspecialidadMedica: 2,
          guid: 'demo-especialidad-2',
          codigo: 'RET-001',
          nombre: 'Retina',
          descripcion: 'Evaluacion especializada de retina.',
          idTblEstadoVigencia: 1,
        },
      ],
      medicos: [
        {
          idMedico: 1,
          guid: 'demo-medico-1',
          idUsuario: 2,
          idEspecialidadMedica: 1,
          cmp: 'CMP12345',
          perfilProfesional: 'Especialista en consultas generales y seguimiento.',
          idTblEstadoVigencia: 1,
        },
      ],
      pacientes: [
        {
          idPaciente: 1,
          guid: 'demo-paciente-1',
          idUsuario: 3,
          fechaNacimiento: '1991-04-18',
          sexo: '1',
          direccion: 'Av. Primavera 123',
          contactoEmergencia: 'Ana Vargas',
          telefonoEmergencia: '988111222',
          idTblEstadoVigencia: 1,
        },
        {
          idPaciente: 2,
          guid: 'demo-paciente-2',
          idUsuario: 4,
          fechaNacimiento: '1988-11-02',
          sexo: '2',
          direccion: 'Jr. Los Olivos 456',
          contactoEmergencia: 'Luis Paredes',
          telefonoEmergencia: '977222333',
          idTblEstadoVigencia: 1,
        },
      ],
      agendasMedicas: [
        {
          idAgendaMedica: 1,
          guid: 'demo-agenda-1',
          idMedico: 1,
          fecha: '2026-05-02',
          horaInicio: '09:00',
          horaFin: '09:30',
          esDisponible: true,
          observacion: 'Turno disponible para consulta general.',
          idTblEstadoVigencia: 1,
        },
        {
          idAgendaMedica: 2,
          guid: 'demo-agenda-2',
          idMedico: 1,
          fecha: '2026-05-02',
          horaInicio: '10:00',
          horaFin: '10:30',
          esDisponible: true,
          observacion: 'Turno disponible para control.',
          idTblEstadoVigencia: 1,
        },
      ],
      citas: [
        {
          idCita: 1,
          guid: 'demo-cita-1',
          idPaciente: 1,
          idMedico: 1,
          idAgendaMedica: 1,
          idEstadoCita: 2,
          idModalidadCita: 1,
          motivo: 'Vision borrosa y control anual.',
          fechaCita: '2026-05-02',
          horaCita: '09:00',
          observacion: 'Paciente requiere dilatacion.',
          idTblEstadoVigencia: 1,
        },
        {
          idCita: 2,
          guid: 'demo-cita-2',
          idPaciente: 2,
          idMedico: 1,
          idAgendaMedica: 2,
          idEstadoCita: 1,
          idModalidadCita: 2,
          motivo: 'Seguimiento de tratamiento.',
          fechaCita: '2026-05-02',
          horaCita: '10:00',
          observacion: 'Revisar evolucion de lentes.',
          idTblEstadoVigencia: 1,
        },
      ],
      pagosCita: [
        {
          idPagoCita: 1,
          guid: 'demo-pago-1',
          idCita: 1,
          idMetodoPago: 2,
          idEstadoPago: 2,
          monto: 150,
          fechaPago: '2026-04-28',
          numeroOperacion: 'OP-1001',
          comprobante: 'B001-000123',
          idTblEstadoVigencia: 1,
        },
      ],
      historialCitas: [
        {
          idHistorialCita: 1,
          guid: 'demo-historial-1',
          idCita: 1,
          idEstadoCita: 2,
          descripcion: 'Cita confirmada por call center.',
          fechaEvento: '2026-04-27',
          idTblEstadoVigencia: 1,
        },
      ],
      recordatoriosCita: [
        {
          idRecordatorioCita: 1,
          guid: 'demo-recordatorio-1',
          idCita: 1,
          idEstadoRecordatorio: 2,
          fechaProgramada: '2026-05-01',
          fechaEnvio: '2026-05-01',
          mensaje: 'Recordatorio de cita para el 02/05 a las 09:00.',
          idTblEstadoVigencia: 1,
        },
      ],
    };
  }
}
