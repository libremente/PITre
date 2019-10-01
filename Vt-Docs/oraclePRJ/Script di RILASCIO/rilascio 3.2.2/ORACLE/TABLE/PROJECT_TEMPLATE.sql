
----------------------
-- PROJECT_TEMPLATE --
----------------------

--DROP TABLE PROJECT_TEMPLATE;
CREATE TABLE PROJECT_TEMPLATE
(
  SYSTEM_ID  INTEGER        NOT NULL,
  NAME       VARCHAR2(255)  NOT NULL
);

ALTER TABLE PROJECT_TEMPLATE ADD (
  CONSTRAINT PROJECT_TEMPLATE_PK
 PRIMARY KEY
 (SYSTEM_ID));
 
CREATE SEQUENCE SEQ_PROJECT_TEMPLATE
    START WITH 1
    MAXVALUE 999999999999999999999999999
    MINVALUE 1
    NOCYCLE
    CACHE 20
    NOORDER;
 
-----------------------
-- PROJECT_STRUCTURE --
-----------------------
 
--DROP TABLE PROJECT_STRUCTURE;
CREATE TABLE PROJECT_STRUCTURE
(
  SYSTEM_ID     INTEGER         NOT NULL,
  ID_PARENT     INTEGER         NULL,
  NAME          VARCHAR2(255)   NOT NULL,
  ID_TEMPLATE   INTEGER         NOT NULL
);

ALTER TABLE PROJECT_STRUCTURE ADD (
  CONSTRAINT PROJECT_STRUCTURE_PK PRIMARY KEY (SYSTEM_ID));

ALTER TABLE PROJECT_STRUCTURE ADD CONSTRAINT FK_STRUCTURE_TEMPLATE
   FOREIGN KEY (ID_TEMPLATE) REFERENCES PROJECT_TEMPLATE (SYSTEM_ID);
   
CREATE SEQUENCE SEQ_PROJECT_STRUCTURE
    START WITH 1
    MAXVALUE 999999999999999999999999999
    MINVALUE 1
    NOCYCLE
    CACHE 20
    NOORDER;
   
---------------------------
-- REL_PROJECT_STRUCTURE --
---------------------------
--DROP TABLE REL_PROJECT_STRUCTURE
CREATE TABLE REL_PROJECT_STRUCTURE
(
    SYSTEM_ID           INTEGER         NOT NULL,
    ID_TIPO_FASCICOLO   INTEGER         NULL,
    ID_TITOLARIO        INTEGER         NULL,
    ID_TEMPLATE         INTEGER         NOT NULL
);

ALTER TABLE REL_PROJECT_STRUCTURE ADD (
  CONSTRAINT REL_PROJECT_STRUCTURE_PK PRIMARY KEY (SYSTEM_ID));

ALTER TABLE REL_PROJECT_STRUCTURE ADD CONSTRAINT FK_REL_PROJECT_TEMPLATE
   FOREIGN KEY (ID_TEMPLATE) REFERENCES PROJECT_TEMPLATE (SYSTEM_ID);

CREATE SEQUENCE SEQ_REL_PROJECT_STRUCTURE
    START WITH 1
    MAXVALUE 999999999999999999999999999
    MINVALUE 1
    NOCYCLE
    CACHE 20
    NOORDER;