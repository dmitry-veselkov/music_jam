CREATE TABLE IF NOT EXISTS public.categories
(
    id_category uuid NOT NULL,
    "order" integer NOT NULL,
    value character varying(512) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT categories_pkey PRIMARY KEY (id_category)
    );

CREATE TABLE IF NOT EXISTS public.games
(
    id uuid NOT NULL,
    id_owner uuid NOT NULL,
    id_pack uuid NOT NULL,
    title character varying(255) COLLATE pg_catalog."default" NOT NULL,
    description character varying(512) COLLATE pg_catalog."default",
    date timestamp with time zone NOT NULL,
                       mode smallint NOT NULL,
                       state smallint NOT NULL,
                       CONSTRAINT games_pkey PRIMARY KEY (id),
    CONSTRAINT games_id_owner_fkey FOREIGN KEY (id_owner)
    REFERENCES public.users (id) MATCH SIMPLE
                   ON UPDATE NO ACTION
                   ON DELETE CASCADE,
    CONSTRAINT games_id_pack_fkey FOREIGN KEY (id_pack)
    REFERENCES public.packs (id_pack) MATCH SIMPLE
                   ON UPDATE NO ACTION
                   ON DELETE NO ACTION
    );

CREATE TABLE IF NOT EXISTS public.pack_round
(
    id_pack uuid NOT NULL,
    id_round uuid NOT NULL,
    "order" integer NOT NULL,
    CONSTRAINT pack_round_pkey PRIMARY KEY (id_pack, id_round),
    CONSTRAINT pack_round_id_pack_order_key UNIQUE (id_pack, "order"),
    CONSTRAINT pack_round_id_pack_fkey FOREIGN KEY (id_pack)
    REFERENCES public.packs (id_pack) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE,
    CONSTRAINT pack_round_id_round_fkey FOREIGN KEY (id_round)
    REFERENCES public.rounds (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE
    );

CREATE TABLE IF NOT EXISTS public.packs
(
    id_pack uuid NOT NULL,
    id_owner uuid NOT NULL,
    title character varying(255) COLLATE pg_catalog."default" NOT NULL,
    created_at_utc timestamp with time zone NOT NULL DEFAULT now(),
    updated_at_utc timestamp with time zone NOT NULL DEFAULT now(),
    CONSTRAINT packs_pkey PRIMARY KEY (id_pack),
    CONSTRAINT packs_id_owner_fkey FOREIGN KEY (id_owner)
    REFERENCES public.users (id) MATCH SIMPLE
                             ON UPDATE NO ACTION
                             ON DELETE CASCADE
    );
CREATE TABLE IF NOT EXISTS public.question_usages
(
    id_question uuid NOT NULL,
    id_pack uuid NOT NULL,
    CONSTRAINT question_usages_pkey PRIMARY KEY (id_question, id_pack),
    CONSTRAINT question_usages_id_pack_fkey FOREIGN KEY (id_pack)
    REFERENCES public.packs (id_pack) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE,
    CONSTRAINT question_usages_id_question_fkey FOREIGN KEY (id_question)
    REFERENCES public.questions (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    );

CREATE TABLE IF NOT EXISTS public.questions
(
    id uuid NOT NULL,
    id_track_q uuid NOT NULL,
    id_track_a uuid NOT NULL,
    requested character varying(512) COLLATE pg_catalog."default" NOT NULL,
    correct_answer character varying(512) COLLATE pg_catalog."default" NOT NULL,
    id_owner uuid NOT NULL,
    CONSTRAINT questions_pkey PRIMARY KEY (id),
    CONSTRAINT questions_id_owner_fkey FOREIGN KEY (id_owner)
    REFERENCES public.users (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION,
    CONSTRAINT questions_id_track_a_fkey FOREIGN KEY (id_track_a)
    REFERENCES public.tracks (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION,
    CONSTRAINT questions_id_track_q_fkey FOREIGN KEY (id_track_q)
    REFERENCES public.tracks (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    );

CREATE TABLE IF NOT EXISTS public.round_question
(
    id_round uuid NOT NULL,
    id_category uuid NOT NULL,
    cost integer NOT NULL,
    question uuid NOT NULL,
    CONSTRAINT round_question_pkey PRIMARY KEY (id_round, id_category, cost),
    CONSTRAINT round_question_id_category_fkey FOREIGN KEY (id_category)
    REFERENCES public.categories (id_category) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION,
    CONSTRAINT round_question_id_round_fkey FOREIGN KEY (id_round)
    REFERENCES public.rounds (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE,
    CONSTRAINT round_question_question_fkey FOREIGN KEY (question)
    REFERENCES public.questions (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    );
CREATE TABLE IF NOT EXISTS public.rounds
(
    id uuid NOT NULL,
    title character varying(255) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT rounds_pkey PRIMARY KEY (id)
    );
CREATE TABLE IF NOT EXISTS public.statistics
(
    id_user uuid NOT NULL,
    wins_count integer NOT NULL DEFAULT 0,
    losses_count integer NOT NULL DEFAULT 0,
    CONSTRAINT statistics_pkey PRIMARY KEY (id_user),
    CONSTRAINT statistics_id_user_fkey FOREIGN KEY (id_user)
    REFERENCES public.users (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE
    );
CREATE TABLE IF NOT EXISTS public.tracks
(
    id uuid NOT NULL,
    link character varying(2048) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT tracks_pkey PRIMARY KEY (id)
    );
CREATE TABLE IF NOT EXISTS public.users
(
    id uuid NOT NULL,
    username character varying(255) COLLATE pg_catalog."default" NOT NULL,
    email character varying(255) COLLATE pg_catalog."default" NOT NULL,
    password character varying(512) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT users_pkey PRIMARY KEY (id),
    CONSTRAINT users_email_key UNIQUE (email),
    CONSTRAINT users_username_key UNIQUE (username)
    );