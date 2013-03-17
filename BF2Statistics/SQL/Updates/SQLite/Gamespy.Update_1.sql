CREATE TABLE "main"."_version"(
  "dbver" INT NOT NULL DEFAULT 0,
  "dbdate" INT NOT NULL DEFAULT 0,
  PRIMARY KEY ("dbver")
);

INSERT INTO "main"."_version" VALUES(2, 0);