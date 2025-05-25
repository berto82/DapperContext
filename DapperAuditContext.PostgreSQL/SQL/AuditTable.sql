-- Table: public.audittable

-- DROP TABLE IF EXISTS public.audittable;

CREATE TABLE IF NOT EXISTS public.audittable
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "Username" character varying(255) COLLATE pg_catalog."default",
    "KeyFieldID" integer,
    "ActionType" integer,
    "DateTimeStamp" timestamp without time zone,
    "DataModel" character varying(255) COLLATE pg_catalog."default",
    "Changes" text COLLATE pg_catalog."default",
    "ValueBefore" text COLLATE pg_catalog."default",
    "ValueAfter" text COLLATE pg_catalog."default",
    CONSTRAINT audittable_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.audittable
    OWNER to postgres;