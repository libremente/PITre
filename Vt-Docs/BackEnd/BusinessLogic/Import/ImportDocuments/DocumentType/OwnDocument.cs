﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using DocsPaVO.utente;
using DocsPaVO.Note;
using DocsPaVO.documento;
using DocsPaVO.rubrica;
using BusinessLogic.Documenti;
using System.Collections;
using DocsPaVO.PrjDocImport;
using System.Globalization;

namespace BusinessLogic.Import.ImportDocuments.DocumentType
{
    /// <summary>
    /// Implementazione dell'importazione per i protocolli interni
    /// </summary>
    class OwnDocument : Document
    {
        public static CultureInfo ci = new CultureInfo("it-IT");
        public static string[] formati = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };
        
        /// <summary>
        /// Funzione per la lettura dei dati da una riga di foglio excel
        /// </summary>
        /// <param name="row">L'oggetto da cui prelevare i dati</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>
        /// L'oggetto con i dati estratti dalla riga del foglio excel
        /// </returns>
        protected override DocumentRowData ReadDataFromCurrentRow(OleDbDataReader row, InfoUtente userInfo, Ruolo role, bool isEnabledPregressi)
        {
            // L'oggetto da restituire
            DocumentRowData toReturn = new DocumentRowData();
            List<string> colonne = new List<string>();
            for (int i = 0; i < row.FieldCount; i++)
                colonne.Add(row.GetName(i));


            // Prelevamento dell'ordinale
            toReturn.OrdinalNumber = row["Ordinale"].ToString().Trim();

            // Prelevamento del codice dell'amministrazione
            toReturn.AdminCode = row["Codice Amministrazione"].ToString().Trim();

            // Prelevamento del numero da assegnare al protocollo e della data di protocollo
            if (isEnabledPregressi)
            {
                if (colonne.Contains("Numero di protocollo"))
                {
                    toReturn.ProtocolNumber = row["Numero di protocollo"].ToString().Trim();
                }

                if (colonne.Contains("Data protocollo"))
                {
                    if (String.IsNullOrEmpty(row["Data protocollo"].ToString()))
                        toReturn.ProtocolDate = DateTime.Now;
                    else
                        toReturn.ProtocolDate = DateTime.Parse(row["Data protocollo"].ToString());
                }

                if (colonne.Contains("Codice Ruolo Creatore"))
                {

                    if (!String.IsNullOrEmpty(row["Codice Utente Creatore"].ToString()))
                    {
                        toReturn.CodiceUtenteCreatore = row["Codice Utente Creatore"].ToString().Trim();
                    }
                }

                if (colonne.Contains("Codice Ruolo Creatore"))
                {

                    if (!String.IsNullOrEmpty(row["Codice Ruolo Creatore"].ToString()))
                    {
                        toReturn.CodiceRuoloCreatore = row["Codice Ruolo Creatore"].ToString().Trim();
                    }
                }
            }
            else
            {

                if (colonne.Contains("Numero di protocollo") ||
                    colonne.Contains("Data protocollo") ||
                     colonne.Contains("Codice Utente Creatore") ||
                     colonne.Contains("Codice Ruolo Creatore")

                    )
                {
                    throw new Exception("Ruolo non abilitato per questa importazione");
                }
            }

            // Prelevamento del codice registro
            toReturn.RegCode = row["Codice Registro"].ToString().Trim();

            // Prelevamento del codice RF
            toReturn.RFCode = row["Codice RF"].ToString().Trim();

            // Prelevamento del codice oggetto
            toReturn.ObjCode = row["Codice oggetto"].ToString().Trim();

            // Prelevamento dell'oggetto
            toReturn.Obj = row["Oggetto"].ToString().Trim();

            // Prelevamento del codice corrispondente, se valorizzato
            if (!String.IsNullOrEmpty(row["Codice corrispondenti"].ToString()))
                toReturn.CorrCode = new List<string>(
                    row["Codice corrispondenti"].ToString().Trim().Split(';'));

            // Prelevamento del pathname nel caso di stampa unione questa colonna non è presente e
            // il valore di pathname viene settato nel blocco cath a stringa vuota
            try
            {

                toReturn.Pathname = row["Pathname"].ToString().Trim();
            }
            catch (Exception e)
            {
                toReturn.Pathname = string.Empty;
            }
            // Prelevamento del valore che indica se il documento deve
            // essere salvato nell'area di lavoro
            toReturn.InWorkingArea = !String.IsNullOrEmpty(row["ADL"].ToString()) &&
                row["ADL"].ToString().Trim().ToUpper() == "SI";

            // Prelevamento delle note
            if (!String.IsNullOrEmpty(row["Note"].ToString()))
            {
                // Creazione della nota
                toReturn.Note = new InfoNota(row["Note"].ToString().Trim());

                //Impostazione della visibilità Tutti
                toReturn.Note.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;

                // Creazione dell'oggetto utilizzato per memorizzare le informazioni 
                // sull'utente creatore
                toReturn.Note.UtenteCreatore = new InfoUtenteCreatoreNota();

                // Impostazione della descrizione del ruolo
                toReturn.Note.UtenteCreatore.DescrizioneRuolo = role.descrizione;

                // Impostazione della descrizione dell'utente
                toReturn.Note.UtenteCreatore.DescrizioneUtente = userInfo.userId;

                // Impostazione dell'id del ruolo
                toReturn.Note.UtenteCreatore.IdRuolo = role.systemId;

                // Impostazione dell'id dell'utente
                toReturn.Note.UtenteCreatore.IdUtente = userInfo.userId;

                // La nota deve essere inserita
                toReturn.Note.DaInserire = true;

                // Impostazione della data di creazione
                toReturn.Note.DataCreazione = DateTime.Now;

            }

            // Prelevamento del codice del modello di trasmissione
            toReturn.TransmissionModelCode = row["Codice modello trasmissione"].ToString().
                Trim().Split(';');

            // Prelevamento della descrizione della tipologia di documento
            toReturn.DocumentTipology = row["Tipologia documento"].ToString().Trim().ToUpper();

            // Prelevamento del codice fascicolo
            if (!String.IsNullOrEmpty(row["Codice fascicolo"].ToString()))
                toReturn.ProjectCodes = row["Codice fascicolo"].ToString().Trim().Split(';');

            // Prelevamento della descrizione del fascicolo
            toReturn.ProjectDescription = row["Descrizione fascicolo"].ToString().Trim();

            // Prelevamento della descrizione del sottofascicolo
            toReturn.FolderDescrition = row["Descrizione sottofascicolo"].ToString().Trim();

            // Prelevamento della descrizione del titolario
            toReturn.Titolario = row["Titolario"].ToString().Trim();

            // Prelevamento del codice del nodo
            toReturn.NodeCode = row["Codice Nodo"].ToString().Trim();

            // Prelevamento della tipologia di fascicolo
            toReturn.ProjectTipology = row["Tipologia fascicolo"].ToString().Trim().ToUpper();

            //Prelevamento del valore che indica se il documento deve essere un predisposto
            if (colonne.Contains("Predisposto"))
            {
                toReturn.Predisposto = !String.IsNullOrEmpty(row["Predisposto"].ToString()) &&
                row["Predisposto"].ToString().Trim().ToUpper() == "SI";
            }

            // Restituzione dell'oggetto con i dati prelevati dalla riga
            return toReturn;

        }

        /// <summary>
        /// Funzione per la validazione dei dati letti da un foglio excel
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati da validare</param>
        /// <param name="isProfilationRequired">True se è obbligatoria la profilazione del documento</param>
        /// <param name="isRapidClassificationRequired">True se è richiesta la classificazione rapida</param>
        /// <param name="validationProblems">La lista dei problemi rilevati in fase di verifica dati</param>
        /// <returns>True se la validazione passa</returns>
        protected override bool CheckDataValidity(DocumentRowData rowData, bool isProfilationRequired, bool isRapidClassificationRequired, out List<string> validationProblems)
        {
            #region Dichiarazione variabili

            // Il risultato della validazione
            bool validationResult = true;

            // L'oggetto a cui richiedere le informaizoni sull'amministrazione
            DocsPaDB.Query_DocsPAWS.Amministrazione administrationManager = null;

            // Valore booleano che indica se è obbligatorio specificare l'RF
            bool rfRequired = false;

            // L'id dell'amministrazione
            string administrationSyd = String.Empty;

            // Valore booleano utilizzato per verificare se solo uno dei due campi
            // Codice Corrispondenti e Corrispondenti è valorizzata
            bool corrOk = true;

            // Valore utilizzato per indicare se è stato trovato un destinatario
            // principale
            bool primCorrFound;

            // Variabile utilizzata per contare il numero di mittenti specificati
            int sendersNum;

            #endregion

            // Creazione della lista dei problemi
            validationProblems = new List<string>();

            // L'ordinale è obbligatorio
            if (String.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                validationResult = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add("Campo 'Numero protocollo emergenza.");
                else
                    validationProblems.Add("Campo 'Ordinale' obbligatorio.");
            }

            // Il codice amministrazione è obbligatorio
            if (String.IsNullOrEmpty(rowData.AdminCode))
            {
                validationResult = false;
                validationProblems.Add("Campo 'Codice amministrazione' obbligatorio.");
            }

            // Il codice registro è obbligatorio
            if (String.IsNullOrEmpty(rowData.RegCode))
            {
                validationResult = false;
                validationProblems.Add("Campo 'Codice Registro' obbligatorio.");
            }

            // Il codice RF deve essere obbligatorio se richiesto dalla segnatura
            // Creazione dell'oggetto a cui richiedere le informazioni sull'amministrazione
            administrationManager = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            try
            {
                administrationSyd = ImportUtils.GetAdministrationId(rowData.AdminCode);
            }
            catch (Exception e)
            {
                validationResult = false;
                validationProblems.Add(String.Format(
                    "Impossibile recuperare le informazioni sull'amministrazione {0}",
                    rowData.AdminCode));
            }

            // Se non è stato possibile ricavare l'id dell'amministrazione, problema
            if (!String.IsNullOrEmpty(administrationSyd))
            {
                // Si procede con la verifica 
                // Verifica dell'obbligatorietà della specifica dell'RF
                rfRequired = administrationManager.AmmGetListAmministrazioni().Where(
                    e => e.IDAmm == administrationSyd).FirstOrDefault().
                        Segnatura.Contains("COD_RF_PROT");

                // Se l'RF è richiesto ma non è specificato, la validazione non passa
                if (rfRequired && String.IsNullOrEmpty(rowData.RFCode))
                {
                    validationResult = false;
                    validationProblems.Add("Campo 'Codice RF' obbligatorio in quanto richiesto dalla segnatura.");
                }

            }

            // Se entrambi i campi 'Oggetto' e 'Codice Oggetto' non
            // sono valorizzati, la validazione non passa
            if (String.IsNullOrEmpty(rowData.ObjCode) &&
                String.IsNullOrEmpty(rowData.Obj))
            {
                validationResult = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add("Il campo 'Oggetto' è obbligatorio.");
                else
                    validationProblems.Add("Non risultano valorizzati nè il campo 'Codice Oggetto' nè il campo 'Oggetto'.  Valorizzare uno dei due.");
            }

            // Se entrambi i campi 'Oggetto' e 'Codice oggetto' sono valorizzati, errore
            if (!String.IsNullOrEmpty(rowData.ObjCode) &&
                !String.IsNullOrEmpty(rowData.Obj))
            {
                validationResult = false;
                validationProblems.Add("Risultano valorizzati entrambi i campi 'Codice Oggetto' e 'Oggetto'. Valorizzare solamente uno.");
            }

            primCorrFound = false;
            sendersNum = 0;
            // Se il campo 'Codice Corrispondenti' è valorizzato ma non contiene #D#,
            // significa che non è stato specificato un destinatario principale e di
            // conseguenza la validazione non passa.
            // Se contine più di un #M#, significa che sono stati specificati più mittenti
            // e di conseguenza la validazione non passa.
            // Se il campo 'Codice Corrispondenti' è valorizzato...
            if (rowData.CorrCode != null)
            {
                // Per ogni codice corrispondente contenuto nella lista...
                foreach (string corrCode in rowData.CorrCode)
                {
                    // ...se contiene #M#, si incrementa il contatore di mittenti
                    if (corrCode.ToUpper().Contains("#M#"))
                        sendersNum++;

                    // ...se contiene #D#, significa che esiste almeno un corrispondente
                    // principale
                    if (corrCode.ToUpper().Contains("#D#") &&
                        corrCode.Trim().Length > 3)
                        primCorrFound = true;

                }

            }

            // Se la validazione dei campi corrispondenti è passata, si procede con
            // la verifica della correttezza dei dati in essi contenuti
            if (corrOk)
            {
                // Se non sono stati individuati mittenti, errore
                if (sendersNum == 0)
                {
                    validationResult = false;
                    validationProblems.Add(
                        "Il campo 'Codice Corrispondenti' non contiene codici per i mittenti. Specificarne uno.");
                }

                // Se sono stati trovati più mittenti, la validazione non passa
                if (sendersNum > 1)
                {
                    validationResult = false;
                    validationProblems.Add(String.Format(
                        "Il campo 'Codice Corrispondenti' contiene {0} mittenti. Specificarne solo uno.",
                        sendersNum));
                }

                // Se non è stato individuato alcun destinatario primario, errore
                if (!primCorrFound)
                {
                    validationResult = false;
                    validationProblems.Add("Nessun destinatario primario specificato nel campo 'Codice Corrispondenti'.");
                }
            }

            // Se il campo 'Codice Fascicolo' del foglio excel è valorizzato, ed
            // è valorizzata anche la profilazione del fascicolo, errore in quanto non è possibile
            // valorizzarli entrambi
            if (rowData.ProjectCodes != null && rowData.ProjectCodes.Length > 0 &&
                (!String.IsNullOrEmpty(rowData.ProjectDescription) ||
                 !String.IsNullOrEmpty(rowData.FolderDescrition) ||
                 !String.IsNullOrEmpty(rowData.NodeCode) ||
                 !String.IsNullOrEmpty(rowData.ProjectTipology)))
            {
                validationResult = false;
                validationProblems.Add("Risultano valorizzati sia i campi 'Codice Fascicolo' che i campi riguardanti la specifica del fascicolo. Non è possibile specificarli entambi.");
            }

            // Se è richiesta profilazione del documento obbligatoria
            // ma non è valorizzato il campo 'Tipologia Documento', la validazione
            // non deve passare
            if (isProfilationRequired && String.IsNullOrEmpty(rowData.DocumentTipology) && !(rowData is RDEDocumentRowData))
            {
                validationResult = false;
                validationProblems.Add("La profilazione del documento è obbligatoria ma non risulta valorizzato il campo 'Tipologia Documento'");
            }

            // Se è richiesta fascicolazione rapida obbligatoria ma non sono specificati 
            // dati in 'Codice Fascicolo' o nei campi dedicati al fascicolo
            if (isRapidClassificationRequired &&
                rowData.ProjectCodes == null &&
                String.IsNullOrEmpty(rowData.ProjectDescription) &&
                String.IsNullOrEmpty(rowData.FolderDescrition) &&
                String.IsNullOrEmpty(rowData.NodeCode) &&
                String.IsNullOrEmpty(rowData.ProjectTipology))
            {
                validationResult = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add("Campo 'Codice Classifica' obbligatorio.");
                else
                    validationProblems.Add("La classificazione è obbligatoria ma non risultano valorizzati nè il campo 'Codice Fascicolo' nè i campi dedicati alla classificazione.");
            }

            // Se il campo Stringa protocollo emergenza non è valorizzato, non si può procedere
            // (Solo nel caso di RDE)
            if (rowData is RDEDocumentRowData && String.IsNullOrEmpty("Stringa protocollo emergenza"))
            {
                validationResult = false;
                validationProblems.Add("Campo 'Stringa protocollo emergenza' obbligatorio.");
            }

            // Se il campo Data protocollo emergenza non è valorizzato, non si può procedere
            // (Solo nel caso di RDE)
            if (rowData is RDEDocumentRowData &&
                String.IsNullOrEmpty(((RDEDocumentRowData)rowData).EmergencyProtocolDate))
            {
                validationResult = false;
                validationProblems.Add("Campo 'Data protocollo emergenza' obbligatorio.");
            }

            // Se il campo Data protocollo emergenza non è valorizzato, non si può procedere
            // (Solo nel caso di RDE)
            if (rowData is RDEDocumentRowData &&
                String.IsNullOrEmpty(((RDEDocumentRowData)rowData).EmergencyProtocolTime))
            {
                validationResult = false;
                validationProblems.Add("Campo 'Ora protocollo emergenza' obbligatorio.");
            }

            if (rowData is RDEDocumentRowData)
            {
                RDEDocumentRowData converted = rowData as RDEDocumentRowData;

                // Il campo Data protocollo emergenza deve essere minore della data di protocollazione, che 
                // si suppone uguale alla data odierna. visto che il protocolla in giallo è stata dismessa 
                // nel senso che non la usa più nessuno.
                if (!String.IsNullOrEmpty(converted.EmergencyProtocolDate) &&
                     !String.IsNullOrEmpty(converted.EmergencyProtocolTime))
                {
                    //l'excel è formattato un modo tale che la data e l'ora possono solo arrivare nel formato valido.
                    string dataEmerg = converted.EmergencyProtocolDate + " " + converted.EmergencyProtocolTime;
                    DateTime dataEmergOut;

                    if (isDate(dataEmerg))
                    {
                        dataEmergOut = DateTime.ParseExact(dataEmerg, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                        if (dataEmergOut > System.DateTime.Now)
                        {
                            validationResult = false;
                            validationProblems.Add("Campo 'Data protocollo emergenza' deve essere minore della data di protocollazione.");
                        }
                    }
                    else
                    {
                        validationResult = false;
                        validationProblems.Add("Il campo data o il campo ora emergenza non contiengo un formato valido.");

                    }

                }
            }

            // Retituzione del risultato di validazione
            return validationResult;

        }

        /// <summary>
        /// Funzione per la creazione dell'oggetto protocollo in Ingresso
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul documenti interno da creare</param>
        /// <param name="registrySyd">Il system id del registro</param>
        /// <param name="rfSyd">Il system id dell'RF</param>
        /// <param name="administrationSyd">Il system id dell'amministrazione</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>L'oggetto con le informazioni sul protocollo da creare</returns>
        protected override Protocollo CreateProtocolObject(DocumentRowData rowData, string registrySyd, string rfSyd, string administrationSyd, bool isSmistamentoEnabled, InfoUtente userInfo, Ruolo role)
        {
            // Creazione dell'oggetto ProtocolloInterno
            ProtocolloInterno ownProto = new ProtocolloInterno();

            // Il corrispondente da inserire
            Corrispondente corr;

            // Il codice corrispondente da analizzare
            // Questo array dovrà contenere due elementi: Il codice corrispondente
            // e la tipologia (M, D, CC)
            string[] corrToAdd;

            // Creazione lista destinatari e destinatari conoscenza
            ownProto.destinatari = new ArrayList();
            ownProto.destinatariConoscenza = new ArrayList();

            // Per ogni codice corrispondente in CorrCode...
            foreach (string corrDesc in rowData.CorrCode)
            {
                // Spezzettamento dei dati sul corrispondente
                corrToAdd = corrDesc.Split('#');

                // Se non ci sono più tre elementi, eccezione
                // Tre elementi poiché il formato con cui è scritto il codice è
                // <Codice>#D|CC#
                if (corrToAdd.Length != 3)
                    throw new Exception(String.Format(
                        "Specifica corrispondente non valida: {0}",
                        corrDesc));

                // A seconda del tipo di corrispondente bisogna intraprendere
                // azioni differenti
                switch (corrToAdd[1].ToUpper().Trim())
                {
                    case "M":       // Mittente del protocollo
                        // Reperimento del corrispondente
                        corr = ImportUtils.GetCorrispondenteByCode(
                            ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_MITT,
                            corrToAdd[0].Trim(),
                            role,
                            userInfo,
                            registrySyd,
                            rfSyd,
                            isSmistamentoEnabled,
                            DocsPaVO.addressbook.TipoUtente.GLOBALE);

                        // Impostazione del mittente, se individuato
                        if(corr != null)
                            ownProto.mittente = corr;
                        break;

                    case "D":       // Destinatario principale
                        // Reperimento del corrispondente
                        corr = ImportUtils.GetCorrispondenteByCode(
                            ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST,
                            corrToAdd[0].Trim(),
                            role,
                            userInfo,
                            registrySyd,
                            rfSyd,
                            isSmistamentoEnabled,
                            DocsPaVO.addressbook.TipoUtente.GLOBALE);

                        // Aggiunta del corrispondente alla lista dei destinatari, se individuato
                        if(corr != null)
                            ownProto.destinatari.Add(corr);
                        break;

                    case "CC":      // Destinatario in copia
                        // Reperimento del corrispondente
                        corr = ImportUtils.GetCorrispondenteByCode(
                            ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST,
                            corrToAdd[0].Trim(),
                            role,
                            userInfo,
                            registrySyd,
                            rfSyd,
                            isSmistamentoEnabled,
                            DocsPaVO.addressbook.TipoUtente.GLOBALE);

                        // Aggiunta del corrispondente alla lista dei destinatari, se individuato
                        if(corr != null)
                            ownProto.destinatariConoscenza.Add(corr);
                        break;

                }

            }

            // Aggiornamento di destinatari, mittenti e destinatari in conoscenza
            ownProto.daAggiornareDestinatari = true;
            ownProto.daAggiornareMittente = true;
            ownProto.daAggiornareDestinatariConoscenza = true;

            // Restituzione dell'oggetto con le informazioni sul protocollo
            return ownProto;
        }

        public static bool isDate(string data)
        {
            try
            {
                data = data.Trim();
                DateTime d_ap = DateTime.ParseExact(data, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}