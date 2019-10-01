CREATE OR REPLACE FUNCTION getInConservazioneDoc(IDPROFILE NUMBER)

RETURN INT IS 
RISULTATO INT;

BEGIN

BEGIN

SELECT COUNT(B.SYSTEM_ID) INTO RISULTATO
FROM DPA_AREA_CONSERVAZIONE A, DPA_ITEMS_CONSERVAZIONE B
WHERE A.SYSTEM_ID=B.ID_CONSERVAZIONE
AND B.ID_PROFILE = IDPROFILE
AND (A.CHA_STATO = 'C' OR A.CHA_STATO = 'V');

IF(RISULTATO>0) THEN
	RISULTATO := 1;
ELSE
	RISULTATO := 0;
END IF;

END;

RETURN RISULTATO;

END getInConservazioneDoc;