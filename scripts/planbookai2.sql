--
-- PostgreSQL database dump
--

\restrict y7Xm4V3AKpxnU7RnI5ZWsXkYnEnz7W99IvOeu8xuIFWpyPOdbaFmgNcvezEMCDM

-- Dumped from database version 17.5 (Debian 17.5-1.pgdg120+1)
-- Dumped by pg_dump version 17.6

-- Started on 2025-09-09 16:38:07

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 9 (class 2615 OID 16387)
-- Name: assessment; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA assessment;


--
-- TOC entry 7 (class 2615 OID 16385)
-- Name: auth; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA auth;


--
-- TOC entry 10 (class 2615 OID 16388)
-- Name: content; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA content;


--
-- TOC entry 12 (class 2615 OID 16390)
-- Name: files; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA files;


--
-- TOC entry 14 (class 2615 OID 16392)
-- Name: logging; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA logging;


--
-- TOC entry 13 (class 2615 OID 16391)
-- Name: notifications; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA notifications;


--
-- TOC entry 11 (class 2615 OID 16389)
-- Name: students; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA students;


--
-- TOC entry 8 (class 2615 OID 16386)
-- Name: users; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA users;


--
-- TOC entry 2 (class 3079 OID 16393)
-- Name: pgcrypto; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS pgcrypto WITH SCHEMA public;


--
-- TOC entry 292 (class 1255 OID 16920)
-- Name: update_updated_at_column(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.update_updated_at_column() RETURNS trigger
    LANGUAGE plpgsql
    AS $$ BEGIN NEW.updated_at = CURRENT_TIMESTAMP; RETURN NEW; END; $$;


SET default_table_access_method = heap;

--
-- TOC entry 239 (class 1259 OID 16627)
-- Name: exam_questions; Type: TABLE; Schema: assessment; Owner: -
--

CREATE TABLE assessment.exam_questions (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    exam_id uuid NOT NULL,
    question_id uuid NOT NULL,
    question_order integer NOT NULL,
    points numeric(4,2) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT exam_questions_points_check CHECK ((points > (0)::numeric))
);


--
-- TOC entry 238 (class 1259 OID 16606)
-- Name: exams; Type: TABLE; Schema: assessment; Owner: -
--

CREATE TABLE assessment.exams (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    title character varying(255) NOT NULL,
    description text,
    subject character varying(100) DEFAULT 'HOA_HOC'::character varying NOT NULL,
    grade integer,
    duration_minutes integer,
    total_score numeric(5,2),
    teacher_id uuid NOT NULL,
    status character varying(50) DEFAULT 'DRAFT'::character varying,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT exams_duration_minutes_check CHECK ((duration_minutes > 0)),
    CONSTRAINT exams_grade_check CHECK ((grade = ANY (ARRAY[10, 11, 12]))),
    CONSTRAINT exams_status_check CHECK (((status)::text = ANY ((ARRAY['DRAFT'::character varying, 'PUBLISHED'::character varying, 'COMPLETED'::character varying, 'ARCHIVED'::character varying])::text[]))),
    CONSTRAINT exams_total_score_check CHECK ((total_score > (0)::numeric))
);


--
-- TOC entry 237 (class 1259 OID 16589)
-- Name: question_choices; Type: TABLE; Schema: assessment; Owner: -
--

CREATE TABLE assessment.question_choices (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    question_id uuid NOT NULL,
    choice_order character(1) NOT NULL,
    content text NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT question_choices_choice_order_check CHECK ((choice_order = ANY (ARRAY['A'::bpchar, 'B'::bpchar, 'C'::bpchar, 'D'::bpchar])))
);


--
-- TOC entry 236 (class 1259 OID 16569)
-- Name: questions; Type: TABLE; Schema: assessment; Owner: -
--

CREATE TABLE assessment.questions (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    content text NOT NULL,
    type character varying(50) NOT NULL,
    difficulty character varying(50),
    subject character varying(100) DEFAULT 'HOA_HOC'::character varying NOT NULL,
    topic character varying(200),
    correct_answer character varying(10),
    explanation text,
    created_by uuid NOT NULL,
    status character varying(50) DEFAULT 'ACTIVE'::character varying,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT questions_difficulty_check CHECK (((difficulty)::text = ANY ((ARRAY['EASY'::character varying, 'MEDIUM'::character varying, 'HARD'::character varying, 'VERY_HARD'::character varying])::text[]))),
    CONSTRAINT questions_status_check CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'INACTIVE'::character varying, 'ARCHIVED'::character varying])::text[]))),
    CONSTRAINT questions_type_check CHECK (((type)::text = ANY ((ARRAY['MULTIPLE_CHOICE'::character varying, 'ESSAY'::character varying, 'SHORT_ANSWER'::character varying, 'TRUE_FALSE'::character varying])::text[])))
);


--
-- TOC entry 230 (class 1259 OID 16478)
-- Name: email_verifications; Type: TABLE; Schema: auth; Owner: -
--

CREATE TABLE auth.email_verifications (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    verification_token character varying(255) NOT NULL,
    expires_at timestamp without time zone NOT NULL,
    is_verified boolean DEFAULT false,
    verified_at timestamp without time zone,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 232 (class 1259 OID 16507)
-- Name: password_history; Type: TABLE; Schema: auth; Owner: -
--

CREATE TABLE auth.password_history (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    password_hash character varying(255) NOT NULL,
    changed_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    changed_by uuid
);


--
-- TOC entry 231 (class 1259 OID 16493)
-- Name: password_resets; Type: TABLE; Schema: auth; Owner: -
--

CREATE TABLE auth.password_resets (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    otp character varying(6) NOT NULL,
    expires_at timestamp without time zone NOT NULL,
    is_used boolean DEFAULT false,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 227 (class 1259 OID 16431)
-- Name: roles; Type: TABLE; Schema: auth; Owner: -
--

CREATE TABLE auth.roles (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description text,
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 226 (class 1259 OID 16430)
-- Name: roles_id_seq; Type: SEQUENCE; Schema: auth; Owner: -
--

CREATE SEQUENCE auth.roles_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 3817 (class 0 OID 0)
-- Dependencies: 226
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: auth; Owner: -
--

ALTER SEQUENCE auth.roles_id_seq OWNED BY auth.roles.id;


--
-- TOC entry 229 (class 1259 OID 16462)
-- Name: sessions; Type: TABLE; Schema: auth; Owner: -
--

CREATE TABLE auth.sessions (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    token character varying(500) NOT NULL,
    expires_at timestamp without time zone NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 233 (class 1259 OID 16519)
-- Name: user_sessions; Type: TABLE; Schema: auth; Owner: -
--

CREATE TABLE auth.user_sessions (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    session_token character varying(255) NOT NULL,
    ip_address inet,
    user_agent text,
    last_activity timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    expires_at timestamp without time zone NOT NULL,
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 228 (class 1259 OID 16444)
-- Name: users; Type: TABLE; Schema: auth; Owner: -
--

CREATE TABLE auth.users (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    email character varying(255) NOT NULL,
    password_hash character varying(255) NOT NULL,
    role_id integer NOT NULL,
    is_active boolean DEFAULT true,
    last_login timestamp without time zone,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    deleted_at timestamp without time zone,
    is_deleted boolean DEFAULT false
);


--
-- TOC entry 242 (class 1259 OID 16699)
-- Name: chu_de; Type: TABLE; Schema: content; Owner: -
--

CREATE TABLE content.chu_de (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    name character varying(255) NOT NULL,
    description text,
    subject character varying(100) DEFAULT 'HOA_HOC'::character varying NOT NULL,
    grade integer,
    parent_id uuid,
    created_by uuid,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chu_de_grade_check CHECK ((grade = ANY (ARRAY[10, 11, 12])))
);


--
-- TOC entry 241 (class 1259 OID 16675)
-- Name: lesson_plans; Type: TABLE; Schema: content; Owner: -
--

CREATE TABLE content.lesson_plans (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    title character varying(255) NOT NULL,
    objectives text,
    content jsonb,
    subject character varying(100) DEFAULT 'HOA_HOC'::character varying NOT NULL,
    grade integer NOT NULL,
    teacher_id uuid NOT NULL,
    template_id uuid,
    status character varying(50) DEFAULT 'DRAFT'::character varying,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    topic_id uuid,
    CONSTRAINT lesson_plans_grade_check CHECK ((grade = ANY (ARRAY[10, 11, 12]))),
    CONSTRAINT lesson_plans_status_check CHECK (((status)::text = ANY ((ARRAY['DRAFT'::character varying, 'COMPLETED'::character varying, 'PUBLISHED'::character varying, 'ARCHIVED'::character varying])::text[])))
);


--
-- TOC entry 240 (class 1259 OID 16656)
-- Name: lesson_templates; Type: TABLE; Schema: content; Owner: -
--

CREATE TABLE content.lesson_templates (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    title character varying(255) NOT NULL,
    description text,
    template_content jsonb,
    subject character varying(100) DEFAULT 'HOA_HOC'::character varying NOT NULL,
    grade integer,
    created_by uuid,
    status character varying(50) DEFAULT 'ACTIVE'::character varying,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT lesson_templates_grade_check CHECK ((grade = ANY (ARRAY[10, 11, 12]))),
    CONSTRAINT lesson_templates_status_check CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'INACTIVE'::character varying, 'ARCHIVED'::character varying])::text[])))
);


--
-- TOC entry 248 (class 1259 OID 16836)
-- Name: file_metadata; Type: TABLE; Schema: files; Owner: -
--

CREATE TABLE files.file_metadata (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    file_id uuid NOT NULL,
    key character varying(100) NOT NULL,
    value text,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 247 (class 1259 OID 16818)
-- Name: file_storage; Type: TABLE; Schema: files; Owner: -
--

CREATE TABLE files.file_storage (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    file_name character varying(255) NOT NULL,
    original_name character varying(255) NOT NULL,
    file_path character varying(500) NOT NULL,
    file_size bigint NOT NULL,
    mime_type character varying(100),
    file_hash character varying(64),
    uploaded_by uuid NOT NULL,
    file_type character varying(50),
    status character varying(50) DEFAULT 'ACTIVE'::character varying,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT file_storage_file_type_check CHECK (((file_type)::text = ANY ((ARRAY['IMAGE'::character varying, 'DOCUMENT'::character varying, 'PDF'::character varying, 'EXCEL'::character varying, 'OTHER'::character varying])::text[]))),
    CONSTRAINT file_storage_status_check CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'ARCHIVED'::character varying, 'DELETED'::character varying])::text[])))
);


--
-- TOC entry 252 (class 1259 OID 16904)
-- Name: performance_metrics; Type: TABLE; Schema: logging; Owner: -
--

CREATE TABLE logging.performance_metrics (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    service_name character varying(100) NOT NULL,
    endpoint character varying(255),
    response_time_ms integer,
    status_code integer,
    user_id uuid,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 251 (class 1259 OID 16889)
-- Name: system_logs; Type: TABLE; Schema: logging; Owner: -
--

CREATE TABLE logging.system_logs (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    level character varying(20) NOT NULL,
    message text NOT NULL,
    source character varying(100),
    user_id uuid,
    ip_address inet,
    user_agent text,
    stack_trace text,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT system_logs_level_check CHECK (((level)::text = ANY ((ARRAY['DEBUG'::character varying, 'INFO'::character varying, 'WARNING'::character varying, 'ERROR'::character varying, 'CRITICAL'::character varying])::text[])))
);


--
-- TOC entry 250 (class 1259 OID 16873)
-- Name: email_queue; Type: TABLE; Schema: notifications; Owner: -
--

CREATE TABLE notifications.email_queue (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    to_email character varying(255) NOT NULL,
    subject character varying(255) NOT NULL,
    body text NOT NULL,
    status character varying(50) DEFAULT 'PENDING'::character varying,
    retry_count integer DEFAULT 0,
    sent_at timestamp without time zone,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT email_queue_status_check CHECK (((status)::text = ANY ((ARRAY['PENDING'::character varying, 'SENT'::character varying, 'FAILED'::character varying])::text[])))
);


--
-- TOC entry 249 (class 1259 OID 16856)
-- Name: notifications; Type: TABLE; Schema: notifications; Owner: -
--

CREATE TABLE notifications.notifications (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    title character varying(255) NOT NULL,
    message text NOT NULL,
    type character varying(50) DEFAULT 'INFO'::character varying,
    is_read boolean DEFAULT false,
    read_at timestamp without time zone,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT notifications_type_check CHECK (((type)::text = ANY ((ARRAY['INFO'::character varying, 'SUCCESS'::character varying, 'WARNING'::character varying, 'ERROR'::character varying])::text[])))
);


--
-- TOC entry 246 (class 1259 OID 16789)
-- Name: answer_sheets; Type: TABLE; Schema: students; Owner: -
--

CREATE TABLE students.answer_sheets (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    student_id uuid NOT NULL,
    exam_id uuid NOT NULL,
    image_url character varying(500),
    ocr_result jsonb,
    ocr_status character varying(50) DEFAULT 'PENDING'::character varying,
    ocr_accuracy numeric(5,2),
    processed_at timestamp without time zone,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT answer_sheets_ocr_status_check CHECK (((ocr_status)::text = ANY ((ARRAY['PENDING'::character varying, 'PROCESSING'::character varying, 'COMPLETED'::character varying, 'FAILED'::character varying])::text[])))
);


--
-- TOC entry 243 (class 1259 OID 16726)
-- Name: classes; Type: TABLE; Schema: students; Owner: -
--

CREATE TABLE students.classes (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    name character varying(100) NOT NULL,
    grade integer NOT NULL,
    student_count integer DEFAULT 0,
    homeroom_teacher_id uuid,
    academic_year character varying(20),
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT classes_grade_check CHECK ((grade = ANY (ARRAY[10, 11, 12]))),
    CONSTRAINT classes_student_count_check CHECK ((student_count >= 0))
);


--
-- TOC entry 245 (class 1259 OID 16765)
-- Name: student_results; Type: TABLE; Schema: students; Owner: -
--

CREATE TABLE students.student_results (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    student_id uuid NOT NULL,
    exam_id uuid NOT NULL,
    score numeric(5,2),
    actual_duration integer,
    answer_details jsonb,
    grading_method character varying(50),
    notes text,
    exam_date timestamp without time zone,
    graded_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT student_results_actual_duration_check CHECK ((actual_duration > 0)),
    CONSTRAINT student_results_grading_method_check CHECK (((grading_method)::text = ANY ((ARRAY['OCR'::character varying, 'MANUAL'::character varying, 'AUTO'::character varying])::text[]))),
    CONSTRAINT student_results_score_check CHECK ((score >= (0)::numeric))
);


--
-- TOC entry 244 (class 1259 OID 16743)
-- Name: students; Type: TABLE; Schema: students; Owner: -
--

CREATE TABLE students.students (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    full_name character varying(255) NOT NULL,
    student_code character varying(50),
    birth_date date,
    gender character varying(10),
    class_id uuid,
    owner_teacher_id uuid NOT NULL,
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT students_gender_check CHECK (((gender)::text = ANY ((ARRAY['MALE'::character varying, 'FEMALE'::character varying, 'OTHER'::character varying])::text[])))
);


--
-- TOC entry 235 (class 1259 OID 16552)
-- Name: activity_logs; Type: TABLE; Schema: users; Owner: -
--

CREATE TABLE users.activity_logs (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    action character varying(100) NOT NULL,
    description text,
    ip_address inet,
    user_agent text,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 253 (class 1259 OID 16933)
-- Name: otp_codes; Type: TABLE; Schema: users; Owner: -
--

CREATE TABLE users.otp_codes (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    otp_code character varying(6) NOT NULL,
    purpose character varying(50) NOT NULL,
    expires_at timestamp without time zone NOT NULL,
    used boolean DEFAULT false,
    attempts integer DEFAULT 0,
    max_attempts integer DEFAULT 3,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT otp_codes_purpose_check CHECK (((purpose)::text = ANY ((ARRAY['PASSWORD_RESET'::character varying, 'EMAIL_VERIFICATION'::character varying, 'PHONE_VERIFICATION'::character varying])::text[])))
);


--
-- TOC entry 254 (class 1259 OID 16949)
-- Name: password_history; Type: TABLE; Schema: users; Owner: -
--

CREATE TABLE users.password_history (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    changed_at timestamp without time zone NOT NULL,
    changed_by uuid,
    reason character varying(100) DEFAULT 'USER_CHANGE'::character varying,
    ip_address inet,
    user_agent text
);


--
-- TOC entry 234 (class 1259 OID 16537)
-- Name: user_profiles; Type: TABLE; Schema: users; Owner: -
--

CREATE TABLE users.user_profiles (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    full_name character varying(255) NOT NULL,
    phone character varying(20),
    address text,
    bio text,
    avatar_url character varying(500),
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 255 (class 1259 OID 16968)
-- Name: user_sessions; Type: TABLE; Schema: users; Owner: -
--

CREATE TABLE users.user_sessions (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    session_token character varying(500) NOT NULL,
    device_info jsonb,
    ip_address inet,
    user_agent text,
    is_active boolean DEFAULT true,
    expires_at timestamp without time zone NOT NULL,
    last_activity timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 3368 (class 2604 OID 16434)
-- Name: roles id; Type: DEFAULT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.roles ALTER COLUMN id SET DEFAULT nextval('auth.roles_id_seq'::regclass);


--
-- TOC entry 3542 (class 2606 OID 16638)
-- Name: exam_questions exam_questions_exam_id_question_id_key; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_question_id_key UNIQUE (exam_id, question_id);


--
-- TOC entry 3544 (class 2606 OID 16636)
-- Name: exam_questions exam_questions_exam_id_question_order_key; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_question_order_key UNIQUE (exam_id, question_order);


--
-- TOC entry 3546 (class 2606 OID 16634)
-- Name: exam_questions exam_questions_pkey; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_pkey PRIMARY KEY (id);


--
-- TOC entry 3538 (class 2606 OID 16621)
-- Name: exams exams_pkey; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_pkey PRIMARY KEY (id);


--
-- TOC entry 3534 (class 2606 OID 16598)
-- Name: question_choices question_choices_pkey; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_pkey PRIMARY KEY (id);


--
-- TOC entry 3536 (class 2606 OID 16600)
-- Name: question_choices question_choices_question_id_choice_order_key; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_question_id_choice_order_key UNIQUE (question_id, choice_order);


--
-- TOC entry 3531 (class 2606 OID 16583)
-- Name: questions questions_pkey; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.questions
    ADD CONSTRAINT questions_pkey PRIMARY KEY (id);


--
-- TOC entry 3508 (class 2606 OID 16485)
-- Name: email_verifications email_verifications_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_pkey PRIMARY KEY (id);


--
-- TOC entry 3510 (class 2606 OID 16487)
-- Name: email_verifications email_verifications_verification_token_key; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_verification_token_key UNIQUE (verification_token);


--
-- TOC entry 3514 (class 2606 OID 16513)
-- Name: password_history password_history_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.password_history
    ADD CONSTRAINT password_history_pkey PRIMARY KEY (id);


--
-- TOC entry 3512 (class 2606 OID 16501)
-- Name: password_resets password_resets_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.password_resets
    ADD CONSTRAINT password_resets_pkey PRIMARY KEY (id);


--
-- TOC entry 3496 (class 2606 OID 16443)
-- Name: roles roles_name_key; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.roles
    ADD CONSTRAINT roles_name_key UNIQUE (name);


--
-- TOC entry 3498 (class 2606 OID 16441)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- TOC entry 3504 (class 2606 OID 16470)
-- Name: sessions sessions_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3506 (class 2606 OID 16472)
-- Name: sessions sessions_token_key; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_token_key UNIQUE (token);


--
-- TOC entry 3516 (class 2606 OID 16529)
-- Name: user_sessions user_sessions_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3518 (class 2606 OID 16531)
-- Name: user_sessions user_sessions_session_token_key; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_session_token_key UNIQUE (session_token);


--
-- TOC entry 3500 (class 2606 OID 16456)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 3502 (class 2606 OID 16454)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 3556 (class 2606 OID 16710)
-- Name: chu_de chu_de_pkey; Type: CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_pkey PRIMARY KEY (id);


--
-- TOC entry 3554 (class 2606 OID 16688)
-- Name: lesson_plans lesson_plans_pkey; Type: CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_pkey PRIMARY KEY (id);


--
-- TOC entry 3549 (class 2606 OID 16669)
-- Name: lesson_templates lesson_templates_pkey; Type: CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_templates
    ADD CONSTRAINT lesson_templates_pkey PRIMARY KEY (id);


--
-- TOC entry 3585 (class 2606 OID 16846)
-- Name: file_metadata file_metadata_file_id_key_key; Type: CONSTRAINT; Schema: files; Owner: -
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_file_id_key_key UNIQUE (file_id, key);


--
-- TOC entry 3587 (class 2606 OID 16844)
-- Name: file_metadata file_metadata_pkey; Type: CONSTRAINT; Schema: files; Owner: -
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_pkey PRIMARY KEY (id);


--
-- TOC entry 3580 (class 2606 OID 16830)
-- Name: file_storage file_storage_pkey; Type: CONSTRAINT; Schema: files; Owner: -
--

ALTER TABLE ONLY files.file_storage
    ADD CONSTRAINT file_storage_pkey PRIMARY KEY (id);


--
-- TOC entry 3604 (class 2606 OID 16910)
-- Name: performance_metrics performance_metrics_pkey; Type: CONSTRAINT; Schema: logging; Owner: -
--

ALTER TABLE ONLY logging.performance_metrics
    ADD CONSTRAINT performance_metrics_pkey PRIMARY KEY (id);


--
-- TOC entry 3600 (class 2606 OID 16898)
-- Name: system_logs system_logs_pkey; Type: CONSTRAINT; Schema: logging; Owner: -
--

ALTER TABLE ONLY logging.system_logs
    ADD CONSTRAINT system_logs_pkey PRIMARY KEY (id);


--
-- TOC entry 3595 (class 2606 OID 16884)
-- Name: email_queue email_queue_pkey; Type: CONSTRAINT; Schema: notifications; Owner: -
--

ALTER TABLE ONLY notifications.email_queue
    ADD CONSTRAINT email_queue_pkey PRIMARY KEY (id);


--
-- TOC entry 3593 (class 2606 OID 16867)
-- Name: notifications notifications_pkey; Type: CONSTRAINT; Schema: notifications; Owner: -
--

ALTER TABLE ONLY notifications.notifications
    ADD CONSTRAINT notifications_pkey PRIMARY KEY (id);


--
-- TOC entry 3576 (class 2606 OID 16799)
-- Name: answer_sheets answer_sheets_pkey; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_pkey PRIMARY KEY (id);


--
-- TOC entry 3560 (class 2606 OID 16737)
-- Name: classes classes_pkey; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.classes
    ADD CONSTRAINT classes_pkey PRIMARY KEY (id);


--
-- TOC entry 3572 (class 2606 OID 16776)
-- Name: student_results student_results_pkey; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_pkey PRIMARY KEY (id);


--
-- TOC entry 3574 (class 2606 OID 16778)
-- Name: student_results student_results_student_id_exam_id_key; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_student_id_exam_id_key UNIQUE (student_id, exam_id);


--
-- TOC entry 3566 (class 2606 OID 16754)
-- Name: students students_owner_teacher_id_student_code_key; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_owner_teacher_id_student_code_key UNIQUE (owner_teacher_id, student_code);


--
-- TOC entry 3568 (class 2606 OID 16752)
-- Name: students students_pkey; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_pkey PRIMARY KEY (id);


--
-- TOC entry 3523 (class 2606 OID 16560)
-- Name: activity_logs activity_logs_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.activity_logs
    ADD CONSTRAINT activity_logs_pkey PRIMARY KEY (id);


--
-- TOC entry 3609 (class 2606 OID 16943)
-- Name: otp_codes otp_codes_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.otp_codes
    ADD CONSTRAINT otp_codes_pkey PRIMARY KEY (id);


--
-- TOC entry 3612 (class 2606 OID 16957)
-- Name: password_history password_history_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_pkey PRIMARY KEY (id);


--
-- TOC entry 3521 (class 2606 OID 16546)
-- Name: user_profiles user_profiles_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.user_profiles
    ADD CONSTRAINT user_profiles_pkey PRIMARY KEY (id);


--
-- TOC entry 3617 (class 2606 OID 16978)
-- Name: user_sessions user_sessions_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3619 (class 2606 OID 16980)
-- Name: user_sessions user_sessions_session_token_key; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_session_token_key UNIQUE (session_token);


--
-- TOC entry 3539 (class 1259 OID 16654)
-- Name: idx_assessment_exams_subject; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_exams_subject ON assessment.exams USING btree (subject);


--
-- TOC entry 3540 (class 1259 OID 16653)
-- Name: idx_assessment_exams_teacher; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_exams_teacher ON assessment.exams USING btree (teacher_id);


--
-- TOC entry 3532 (class 1259 OID 16655)
-- Name: idx_assessment_question_choices_question; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_question_choices_question ON assessment.question_choices USING btree (question_id);


--
-- TOC entry 3526 (class 1259 OID 16652)
-- Name: idx_assessment_questions_created_by; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_questions_created_by ON assessment.questions USING btree (created_by);


--
-- TOC entry 3527 (class 1259 OID 16651)
-- Name: idx_assessment_questions_difficulty; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_questions_difficulty ON assessment.questions USING btree (difficulty);


--
-- TOC entry 3528 (class 1259 OID 16649)
-- Name: idx_assessment_questions_subject; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_questions_subject ON assessment.questions USING btree (subject);


--
-- TOC entry 3529 (class 1259 OID 16650)
-- Name: idx_assessment_questions_topic; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_questions_topic ON assessment.questions USING btree (topic);


--
-- TOC entry 3557 (class 1259 OID 16725)
-- Name: idx_content_chu_de_parent; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_content_chu_de_parent ON content.chu_de USING btree (parent_id);


--
-- TOC entry 3558 (class 1259 OID 16724)
-- Name: idx_content_chu_de_subject; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_content_chu_de_subject ON content.chu_de USING btree (subject);


--
-- TOC entry 3550 (class 1259 OID 16723)
-- Name: idx_content_lesson_plans_subject; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_content_lesson_plans_subject ON content.lesson_plans USING btree (subject);


--
-- TOC entry 3551 (class 1259 OID 16722)
-- Name: idx_content_lesson_plans_teacher; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_content_lesson_plans_teacher ON content.lesson_plans USING btree (teacher_id);


--
-- TOC entry 3547 (class 1259 OID 16721)
-- Name: idx_content_lesson_templates_subject; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_content_lesson_templates_subject ON content.lesson_templates USING btree (subject);


--
-- TOC entry 3552 (class 1259 OID 17000)
-- Name: idx_lesson_plans_topic; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_lesson_plans_topic ON content.lesson_plans USING btree (topic_id);


--
-- TOC entry 3588 (class 1259 OID 16855)
-- Name: idx_files_metadata_file; Type: INDEX; Schema: files; Owner: -
--

CREATE INDEX idx_files_metadata_file ON files.file_metadata USING btree (file_id);


--
-- TOC entry 3581 (class 1259 OID 16853)
-- Name: idx_files_storage_file_type; Type: INDEX; Schema: files; Owner: -
--

CREATE INDEX idx_files_storage_file_type ON files.file_storage USING btree (file_type);


--
-- TOC entry 3582 (class 1259 OID 16854)
-- Name: idx_files_storage_status; Type: INDEX; Schema: files; Owner: -
--

CREATE INDEX idx_files_storage_status ON files.file_storage USING btree (status);


--
-- TOC entry 3583 (class 1259 OID 16852)
-- Name: idx_files_storage_uploaded_by; Type: INDEX; Schema: files; Owner: -
--

CREATE INDEX idx_files_storage_uploaded_by ON files.file_storage USING btree (uploaded_by);


--
-- TOC entry 3601 (class 1259 OID 16919)
-- Name: idx_logging_performance_created_at; Type: INDEX; Schema: logging; Owner: -
--

CREATE INDEX idx_logging_performance_created_at ON logging.performance_metrics USING btree (created_at);


--
-- TOC entry 3602 (class 1259 OID 16918)
-- Name: idx_logging_performance_service; Type: INDEX; Schema: logging; Owner: -
--

CREATE INDEX idx_logging_performance_service ON logging.performance_metrics USING btree (service_name);


--
-- TOC entry 3597 (class 1259 OID 16917)
-- Name: idx_logging_system_logs_created_at; Type: INDEX; Schema: logging; Owner: -
--

CREATE INDEX idx_logging_system_logs_created_at ON logging.system_logs USING btree (created_at);


--
-- TOC entry 3598 (class 1259 OID 16916)
-- Name: idx_logging_system_logs_level; Type: INDEX; Schema: logging; Owner: -
--

CREATE INDEX idx_logging_system_logs_level ON logging.system_logs USING btree (level);


--
-- TOC entry 3596 (class 1259 OID 16888)
-- Name: idx_email_queue_status; Type: INDEX; Schema: notifications; Owner: -
--

CREATE INDEX idx_email_queue_status ON notifications.email_queue USING btree (status);


--
-- TOC entry 3589 (class 1259 OID 16887)
-- Name: idx_notifications_created_at; Type: INDEX; Schema: notifications; Owner: -
--

CREATE INDEX idx_notifications_created_at ON notifications.notifications USING btree (created_at);


--
-- TOC entry 3590 (class 1259 OID 16886)
-- Name: idx_notifications_is_read; Type: INDEX; Schema: notifications; Owner: -
--

CREATE INDEX idx_notifications_is_read ON notifications.notifications USING btree (is_read);


--
-- TOC entry 3591 (class 1259 OID 16885)
-- Name: idx_notifications_user; Type: INDEX; Schema: notifications; Owner: -
--

CREATE INDEX idx_notifications_user ON notifications.notifications USING btree (user_id);


--
-- TOC entry 3577 (class 1259 OID 16817)
-- Name: idx_students_answer_sheets_exam; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_answer_sheets_exam ON students.answer_sheets USING btree (exam_id);


--
-- TOC entry 3578 (class 1259 OID 16816)
-- Name: idx_students_answer_sheets_student; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_answer_sheets_student ON students.answer_sheets USING btree (student_id);


--
-- TOC entry 3561 (class 1259 OID 16810)
-- Name: idx_students_classes_teacher; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_classes_teacher ON students.classes USING btree (homeroom_teacher_id);


--
-- TOC entry 3569 (class 1259 OID 16815)
-- Name: idx_students_student_results_exam; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_student_results_exam ON students.student_results USING btree (exam_id);


--
-- TOC entry 3570 (class 1259 OID 16814)
-- Name: idx_students_student_results_student; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_student_results_student ON students.student_results USING btree (student_id);


--
-- TOC entry 3562 (class 1259 OID 16811)
-- Name: idx_students_students_class; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_students_class ON students.students USING btree (class_id);


--
-- TOC entry 3563 (class 1259 OID 16813)
-- Name: idx_students_students_code; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_students_code ON students.students USING btree (student_code);


--
-- TOC entry 3564 (class 1259 OID 16812)
-- Name: idx_students_students_teacher; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_students_teacher ON students.students USING btree (owner_teacher_id);


--
-- TOC entry 3524 (class 1259 OID 16568)
-- Name: idx_users_activity_logs_created; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_activity_logs_created ON users.activity_logs USING btree (created_at);


--
-- TOC entry 3525 (class 1259 OID 16567)
-- Name: idx_users_activity_logs_user; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_activity_logs_user ON users.activity_logs USING btree (user_id);


--
-- TOC entry 3605 (class 1259 OID 16988)
-- Name: idx_users_otp_codes_expires; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_otp_codes_expires ON users.otp_codes USING btree (expires_at);


--
-- TOC entry 3606 (class 1259 OID 16987)
-- Name: idx_users_otp_codes_purpose; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_otp_codes_purpose ON users.otp_codes USING btree (purpose);


--
-- TOC entry 3607 (class 1259 OID 16986)
-- Name: idx_users_otp_codes_user; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_otp_codes_user ON users.otp_codes USING btree (user_id);


--
-- TOC entry 3610 (class 1259 OID 16989)
-- Name: idx_users_password_history_user; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_password_history_user ON users.password_history USING btree (user_id);


--
-- TOC entry 3519 (class 1259 OID 16566)
-- Name: idx_users_profiles_user; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_profiles_user ON users.user_profiles USING btree (user_id);


--
-- TOC entry 3613 (class 1259 OID 16992)
-- Name: idx_users_user_sessions_active; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_user_sessions_active ON users.user_sessions USING btree (is_active);


--
-- TOC entry 3614 (class 1259 OID 16991)
-- Name: idx_users_user_sessions_token; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_user_sessions_token ON users.user_sessions USING btree (session_token);


--
-- TOC entry 3615 (class 1259 OID 16990)
-- Name: idx_users_user_sessions_user; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_user_sessions_user ON users.user_sessions USING btree (user_id);


--
-- TOC entry 3660 (class 2620 OID 16925)
-- Name: exams trigger_updated_at_assessment_exams; Type: TRIGGER; Schema: assessment; Owner: -
--

CREATE TRIGGER trigger_updated_at_assessment_exams BEFORE UPDATE ON assessment.exams FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3659 (class 2620 OID 16924)
-- Name: questions trigger_updated_at_assessment_questions; Type: TRIGGER; Schema: assessment; Owner: -
--

CREATE TRIGGER trigger_updated_at_assessment_questions BEFORE UPDATE ON assessment.questions FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3655 (class 2620 OID 16921)
-- Name: roles trigger_updated_at_auth_roles; Type: TRIGGER; Schema: auth; Owner: -
--

CREATE TRIGGER trigger_updated_at_auth_roles BEFORE UPDATE ON auth.roles FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3656 (class 2620 OID 16922)
-- Name: users trigger_updated_at_auth_users; Type: TRIGGER; Schema: auth; Owner: -
--

CREATE TRIGGER trigger_updated_at_auth_users BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3657 (class 2620 OID 16993)
-- Name: users trigger_updated_at_auth_users_soft_delete; Type: TRIGGER; Schema: auth; Owner: -
--

CREATE TRIGGER trigger_updated_at_auth_users_soft_delete BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3663 (class 2620 OID 16928)
-- Name: chu_de trigger_updated_at_content_chu_de; Type: TRIGGER; Schema: content; Owner: -
--

CREATE TRIGGER trigger_updated_at_content_chu_de BEFORE UPDATE ON content.chu_de FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3662 (class 2620 OID 16927)
-- Name: lesson_plans trigger_updated_at_content_lesson_plans; Type: TRIGGER; Schema: content; Owner: -
--

CREATE TRIGGER trigger_updated_at_content_lesson_plans BEFORE UPDATE ON content.lesson_plans FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3661 (class 2620 OID 16926)
-- Name: lesson_templates trigger_updated_at_content_lesson_templates; Type: TRIGGER; Schema: content; Owner: -
--

CREATE TRIGGER trigger_updated_at_content_lesson_templates BEFORE UPDATE ON content.lesson_templates FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3666 (class 2620 OID 16931)
-- Name: file_storage trigger_updated_at_files_file_storage; Type: TRIGGER; Schema: files; Owner: -
--

CREATE TRIGGER trigger_updated_at_files_file_storage BEFORE UPDATE ON files.file_storage FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3664 (class 2620 OID 16929)
-- Name: classes trigger_updated_at_students_classes; Type: TRIGGER; Schema: students; Owner: -
--

CREATE TRIGGER trigger_updated_at_students_classes BEFORE UPDATE ON students.classes FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3665 (class 2620 OID 16930)
-- Name: students trigger_updated_at_students_students; Type: TRIGGER; Schema: students; Owner: -
--

CREATE TRIGGER trigger_updated_at_students_students BEFORE UPDATE ON students.students FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3658 (class 2620 OID 16923)
-- Name: user_profiles trigger_updated_at_users_user_profiles; Type: TRIGGER; Schema: users; Owner: -
--

CREATE TRIGGER trigger_updated_at_users_user_profiles BEFORE UPDATE ON users.user_profiles FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3631 (class 2606 OID 16639)
-- Name: exam_questions exam_questions_exam_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id) ON DELETE CASCADE;


--
-- TOC entry 3632 (class 2606 OID 16644)
-- Name: exam_questions exam_questions_question_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_question_id_fkey FOREIGN KEY (question_id) REFERENCES assessment.questions(id);


--
-- TOC entry 3630 (class 2606 OID 16622)
-- Name: exams exams_teacher_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3629 (class 2606 OID 16601)
-- Name: question_choices question_choices_question_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_question_id_fkey FOREIGN KEY (question_id) REFERENCES assessment.questions(id) ON DELETE CASCADE;


--
-- TOC entry 3628 (class 2606 OID 16584)
-- Name: questions questions_created_by_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.questions
    ADD CONSTRAINT questions_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3622 (class 2606 OID 16488)
-- Name: email_verifications email_verifications_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3624 (class 2606 OID 16514)
-- Name: password_history password_history_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.password_history
    ADD CONSTRAINT password_history_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3623 (class 2606 OID 16502)
-- Name: password_resets password_resets_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.password_resets
    ADD CONSTRAINT password_resets_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3621 (class 2606 OID 16473)
-- Name: sessions sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3625 (class 2606 OID 16532)
-- Name: user_sessions user_sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3620 (class 2606 OID 16457)
-- Name: users users_role_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES auth.roles(id);


--
-- TOC entry 3637 (class 2606 OID 16716)
-- Name: chu_de chu_de_created_by_fkey; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3638 (class 2606 OID 16711)
-- Name: chu_de chu_de_parent_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_parent_id_fkey FOREIGN KEY (parent_id) REFERENCES content.chu_de(id);


--
-- TOC entry 3634 (class 2606 OID 16995)
-- Name: lesson_plans fk_lesson_plans_topic; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT fk_lesson_plans_topic FOREIGN KEY (topic_id) REFERENCES content.chu_de(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 3635 (class 2606 OID 16689)
-- Name: lesson_plans lesson_plans_teacher_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3636 (class 2606 OID 16694)
-- Name: lesson_plans lesson_plans_template_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_template_id_fkey FOREIGN KEY (template_id) REFERENCES content.lesson_templates(id);


--
-- TOC entry 3633 (class 2606 OID 16670)
-- Name: lesson_templates lesson_templates_created_by_fkey; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_templates
    ADD CONSTRAINT lesson_templates_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3647 (class 2606 OID 16847)
-- Name: file_metadata file_metadata_file_id_fkey; Type: FK CONSTRAINT; Schema: files; Owner: -
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_file_id_fkey FOREIGN KEY (file_id) REFERENCES files.file_storage(id) ON DELETE CASCADE;


--
-- TOC entry 3646 (class 2606 OID 16831)
-- Name: file_storage file_storage_uploaded_by_fkey; Type: FK CONSTRAINT; Schema: files; Owner: -
--

ALTER TABLE ONLY files.file_storage
    ADD CONSTRAINT file_storage_uploaded_by_fkey FOREIGN KEY (uploaded_by) REFERENCES auth.users(id);


--
-- TOC entry 3650 (class 2606 OID 16911)
-- Name: performance_metrics performance_metrics_user_id_fkey; Type: FK CONSTRAINT; Schema: logging; Owner: -
--

ALTER TABLE ONLY logging.performance_metrics
    ADD CONSTRAINT performance_metrics_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3649 (class 2606 OID 16899)
-- Name: system_logs system_logs_user_id_fkey; Type: FK CONSTRAINT; Schema: logging; Owner: -
--

ALTER TABLE ONLY logging.system_logs
    ADD CONSTRAINT system_logs_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3648 (class 2606 OID 16868)
-- Name: notifications notifications_user_id_fkey; Type: FK CONSTRAINT; Schema: notifications; Owner: -
--

ALTER TABLE ONLY notifications.notifications
    ADD CONSTRAINT notifications_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3644 (class 2606 OID 16805)
-- Name: answer_sheets answer_sheets_exam_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id);


--
-- TOC entry 3645 (class 2606 OID 16800)
-- Name: answer_sheets answer_sheets_student_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_student_id_fkey FOREIGN KEY (student_id) REFERENCES students.students(id);


--
-- TOC entry 3639 (class 2606 OID 16738)
-- Name: classes classes_homeroom_teacher_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.classes
    ADD CONSTRAINT classes_homeroom_teacher_id_fkey FOREIGN KEY (homeroom_teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3642 (class 2606 OID 16784)
-- Name: student_results student_results_exam_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id);


--
-- TOC entry 3643 (class 2606 OID 16779)
-- Name: student_results student_results_student_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_student_id_fkey FOREIGN KEY (student_id) REFERENCES students.students(id);


--
-- TOC entry 3640 (class 2606 OID 16755)
-- Name: students students_class_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_class_id_fkey FOREIGN KEY (class_id) REFERENCES students.classes(id);


--
-- TOC entry 3641 (class 2606 OID 16760)
-- Name: students students_owner_teacher_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_owner_teacher_id_fkey FOREIGN KEY (owner_teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3627 (class 2606 OID 16561)
-- Name: activity_logs activity_logs_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.activity_logs
    ADD CONSTRAINT activity_logs_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3651 (class 2606 OID 16944)
-- Name: otp_codes otp_codes_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.otp_codes
    ADD CONSTRAINT otp_codes_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3652 (class 2606 OID 16963)
-- Name: password_history password_history_changed_by_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_changed_by_fkey FOREIGN KEY (changed_by) REFERENCES auth.users(id);


--
-- TOC entry 3653 (class 2606 OID 16958)
-- Name: password_history password_history_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3626 (class 2606 OID 16547)
-- Name: user_profiles user_profiles_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.user_profiles
    ADD CONSTRAINT user_profiles_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3654 (class 2606 OID 16981)
-- Name: user_sessions user_sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


-- Completed on 2025-09-09 16:38:07

--
-- PostgreSQL database dump complete
--

\unrestrict y7Xm4V3AKpxnU7RnI5ZWsXkYnEnz7W99IvOeu8xuIFWpyPOdbaFmgNcvezEMCDM

