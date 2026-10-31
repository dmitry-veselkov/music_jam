CREATE TABLE categories (
    id uuid NOT NULL,
    "order" integer NOT NULL,
    name character varying(120) NOT NULL,
    CONSTRAINT pk_categories PRIMARY KEY (id)
);

CREATE TABLE rounds (
    id uuid NOT NULL,
    title character varying(255) NOT NULL,
    CONSTRAINT pk_rounds PRIMARY KEY (id)
);

CREATE TABLE tracks (
    id uuid NOT NULL,
    link text NOT NULL,
    CONSTRAINT pk_tracks PRIMARY KEY (id)
);

CREATE TABLE users (
    id uuid NOT NULL,
    username text NOT NULL,
    email text NOT NULL,
    password text NOT NULL,
    CONSTRAINT pk_users PRIMARY KEY (id),
    CONSTRAINT uq_users_email UNIQUE (email),
    CONSTRAINT uq_users_username UNIQUE (username)
);

CREATE TABLE packs (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    title character varying(255) NOT NULL,
    created_at_utc timestamp with time zone NOT NULL DEFAULT (now()),
    updated_at_utc timestamp with time zone NOT NULL DEFAULT (now()),
    CONSTRAINT pk_packs PRIMARY KEY (id),
    CONSTRAINT fk_packs_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE questions (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    question_track_id uuid NOT NULL,
    answer_track_id uuid NOT NULL,
    requested text NOT NULL,
    correct_answer text NOT NULL,
    CONSTRAINT pk_questions PRIMARY KEY (id),
    CONSTRAINT fk_questions_tracks_answer_track_id FOREIGN KEY (answer_track_id) REFERENCES tracks (id) ON DELETE CASCADE,
    CONSTRAINT fk_questions_tracks_question_track_id FOREIGN KEY (question_track_id) REFERENCES tracks (id) ON DELETE CASCADE,
    CONSTRAINT fk_questions_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE statistics (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    wins_count integer NOT NULL,
    losses_count integer NOT NULL,
    total_count integer NOT NULL,
    CONSTRAINT pk_statistics PRIMARY KEY (id),
    CONSTRAINT fk_statistics_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE games (
    id uuid NOT NULL,
    pack_id uuid NOT NULL,
    user_id uuid NOT NULL,
    title character varying(255) NOT NULL,
    description character varying(512) NOT NULL,
    game_time timestamp with time zone NOT NULL,
    mode integer NOT NULL,
    state integer NOT NULL,
    CONSTRAINT pk_games PRIMARY KEY (id),
    CONSTRAINT fk_games_packs_pack_id FOREIGN KEY (pack_id) REFERENCES packs (id) ON DELETE CASCADE,
    CONSTRAINT fk_games_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE pack_round (
    pack_id uuid NOT NULL,
    round_id uuid NOT NULL,
    "order" integer NOT NULL,
    CONSTRAINT pk_pack_round PRIMARY KEY (pack_id, round_id),
    CONSTRAINT fk_pack_round_packs_pack_id FOREIGN KEY (pack_id) REFERENCES packs (id) ON DELETE CASCADE,
    CONSTRAINT fk_pack_round_rounds_round_id FOREIGN KEY (round_id) REFERENCES rounds (id) ON DELETE CASCADE
);

CREATE TABLE question_usage (
    id uuid NOT NULL,
    question_id uuid NOT NULL,
    pack_id uuid NOT NULL,
    CONSTRAINT pk_question_usage PRIMARY KEY (id),
    CONSTRAINT fk_question_usage_packs_pack_id FOREIGN KEY (pack_id) REFERENCES packs (id) ON DELETE CASCADE,
    CONSTRAINT fk_question_usage_questions_question_id FOREIGN KEY (question_id) REFERENCES questions (id) ON DELETE CASCADE
);

CREATE TABLE round_question (
    round_id uuid NOT NULL,
    category_id uuid NOT NULL,
    cost integer NOT NULL,
    question_id uuid,
    CONSTRAINT pk_round_question PRIMARY KEY (round_id, category_id, cost),
    CONSTRAINT fk_round_question_categories_category_id FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE CASCADE,
    CONSTRAINT fk_round_question_questions_question_id FOREIGN KEY (question_id) REFERENCES questions (id),
    CONSTRAINT fk_round_question_rounds_round_id FOREIGN KEY (round_id) REFERENCES rounds (id) ON DELETE CASCADE
);

CREATE INDEX ix_games_pack_id ON games (pack_id);

CREATE INDEX ix_games_user_id ON games (user_id);

CREATE INDEX ix_pack_round_round_id ON pack_round (round_id);

CREATE INDEX ix_packs_user_id ON packs (user_id);

CREATE INDEX ix_question_usage_pack_id ON question_usage (pack_id);

CREATE INDEX ix_question_usage_question_id ON question_usage (question_id);

CREATE INDEX ix_questions_answer_track_id ON questions (answer_track_id);

CREATE INDEX ix_questions_question_track_id ON questions (question_track_id);

CREATE INDEX ix_questions_user_id ON questions (user_id);

CREATE INDEX ix_round_question_category_id ON round_question (category_id);

CREATE INDEX ix_round_question_question_id ON round_question (question_id);

CREATE UNIQUE INDEX ix_statistics_user_id ON statistics (user_id);
