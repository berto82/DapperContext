CREATE TABLE AuditTable (
    ID            INTEGER    PRIMARY KEY AUTOINCREMENT
                             NOT NULL
                             UNIQUE,
    Username      TEXT (255) NOT NULL,
    KeyFieldID    INTEGER    NOT NULL,
    ActionType    INTEGER    NOT NULL,
    DateTimeStamp REAL       NOT NULL,
    DataModel     TEXT       NOT NULL,
    Changes       TEXT       NOT NULL,
    ValueBefore   TEXT       NOT NULL,
    ValueAfter    TEXT       NOT NULL
);
