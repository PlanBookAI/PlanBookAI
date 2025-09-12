--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5 (Debian 17.5-1.pgdg120+1)
-- Dumped by pg_dump version 17.5

-- Started on 2025-09-12 21:34:55

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

DROP DATABASE IF EXISTS planbookai;
--
-- TOC entry 3848 (class 1262 OID 24643)
-- Name: planbookai; Type: DATABASE; Schema: -; Owner: test
--

CREATE DATABASE planbookai WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE planbookai OWNER TO test;

\connect planbookai

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
-- TOC entry 9 (class 2615 OID 24644)
-- Name: assessment; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA assessment;


ALTER SCHEMA assessment OWNER TO test;

--
-- TOC entry 7 (class 2615 OID 24645)
-- Name: auth; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA auth;


ALTER SCHEMA auth OWNER TO test;

--
-- TOC entry 10 (class 2615 OID 24646)
-- Name: content; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA content;


ALTER SCHEMA content OWNER TO test;

--
-- TOC entry 12 (class 2615 OID 24647)
-- Name: files; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA files;


ALTER SCHEMA files OWNER TO test;

--
-- TOC entry 14 (class 2615 OID 24648)
-- Name: logging; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA logging;


ALTER SCHEMA logging OWNER TO test;

--
-- TOC entry 13 (class 2615 OID 24649)
-- Name: notifications; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA notifications;


ALTER SCHEMA notifications OWNER TO test;

--
-- TOC entry 5 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: pg_database_owner
--

CREATE SCHEMA public;


ALTER SCHEMA public OWNER TO pg_database_owner;

--
-- TOC entry 3849 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: pg_database_owner
--

COMMENT ON SCHEMA public IS 'standard public schema';


--
-- TOC entry 11 (class 2615 OID 24650)
-- Name: students; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA students;


ALTER SCHEMA students OWNER TO test;

--
-- TOC entry 8 (class 2615 OID 24651)
-- Name: users; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA users;


ALTER SCHEMA users OWNER TO test;

--
-- TOC entry 292 (class 1255 OID 24689)
-- Name: update_updated_at_column(); Type: FUNCTION; Schema: public; Owner: test
--

CREATE FUNCTION public.update_updated_at_column() RETURNS trigger
    LANGUAGE plpgsql
    AS $$ BEGIN NEW.updated_at = CURRENT_TIMESTAMP; RETURN NEW; END; $$;


ALTER FUNCTION public.update_updated_at_column() OWNER TO test;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 226 (class 1259 OID 24690)
-- Name: exam_questions; Type: TABLE; Schema: assessment; Owner: test
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


ALTER TABLE assessment.exam_questions OWNER TO test;

--
-- TOC entry 227 (class 1259 OID 24696)
-- Name: exams; Type: TABLE; Schema: assessment; Owner: test
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
    CONSTRAINT exams_status_check CHECK (((status)::text = ANY (ARRAY[('DRAFT'::character varying)::text, ('PUBLISHED'::character varying)::text, ('COMPLETED'::character varying)::text, ('ARCHIVED'::character varying)::text]))),
    CONSTRAINT exams_total_score_check CHECK ((total_score > (0)::numeric))
);


ALTER TABLE assessment.exams OWNER TO test;

--
-- TOC entry 228 (class 1259 OID 24710)
-- Name: question_choices; Type: TABLE; Schema: assessment; Owner: test
--

CREATE TABLE assessment.question_choices (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    question_id uuid NOT NULL,
    choice_order character(1) NOT NULL,
    content text NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT question_choices_choice_order_check CHECK ((choice_order = ANY (ARRAY['A'::bpchar, 'B'::bpchar, 'C'::bpchar, 'D'::bpchar])))
);


ALTER TABLE assessment.question_choices OWNER TO test;

--
-- TOC entry 229 (class 1259 OID 24718)
-- Name: questions; Type: TABLE; Schema: assessment; Owner: test
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
    points numeric(5,2) DEFAULT 1.0,
    CONSTRAINT questions_difficulty_check CHECK (((difficulty)::text = ANY (ARRAY[('EASY'::character varying)::text, ('MEDIUM'::character varying)::text, ('HARD'::character varying)::text, ('VERY_HARD'::character varying)::text]))),
    CONSTRAINT questions_status_check CHECK (((status)::text = ANY (ARRAY[('ACTIVE'::character varying)::text, ('INACTIVE'::character varying)::text, ('ARCHIVED'::character varying)::text]))),
    CONSTRAINT questions_type_check CHECK (((type)::text = ANY (ARRAY[('MULTIPLE_CHOICE'::character varying)::text, ('ESSAY'::character varying)::text, ('SHORT_ANSWER'::character varying)::text, ('TRUE_FALSE'::character varying)::text])))
);


ALTER TABLE assessment.questions OWNER TO test;

--
-- TOC entry 230 (class 1259 OID 24731)
-- Name: email_verifications; Type: TABLE; Schema: auth; Owner: test
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


ALTER TABLE auth.email_verifications OWNER TO test;

--
-- TOC entry 231 (class 1259 OID 24737)
-- Name: password_history; Type: TABLE; Schema: auth; Owner: test
--

CREATE TABLE auth.password_history (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    password_hash character varying(255) NOT NULL,
    changed_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    changed_by uuid
);


ALTER TABLE auth.password_history OWNER TO test;

--
-- TOC entry 232 (class 1259 OID 24742)
-- Name: password_resets; Type: TABLE; Schema: auth; Owner: test
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


ALTER TABLE auth.password_resets OWNER TO test;

--
-- TOC entry 233 (class 1259 OID 24749)
-- Name: roles; Type: TABLE; Schema: auth; Owner: test
--

CREATE TABLE auth.roles (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description text,
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE auth.roles OWNER TO test;

--
-- TOC entry 234 (class 1259 OID 24757)
-- Name: roles_id_seq; Type: SEQUENCE; Schema: auth; Owner: test
--

CREATE SEQUENCE auth.roles_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE auth.roles_id_seq OWNER TO test;

--
-- TOC entry 3850 (class 0 OID 0)
-- Dependencies: 234
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: auth; Owner: test
--

ALTER SEQUENCE auth.roles_id_seq OWNED BY auth.roles.id;


--
-- TOC entry 235 (class 1259 OID 24758)
-- Name: sessions; Type: TABLE; Schema: auth; Owner: test
--

CREATE TABLE auth.sessions (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    token character varying(500) NOT NULL,
    expires_at timestamp without time zone NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE auth.sessions OWNER TO test;

--
-- TOC entry 236 (class 1259 OID 24765)
-- Name: user_sessions; Type: TABLE; Schema: auth; Owner: test
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


ALTER TABLE auth.user_sessions OWNER TO test;

--
-- TOC entry 237 (class 1259 OID 24774)
-- Name: users; Type: TABLE; Schema: auth; Owner: test
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


ALTER TABLE auth.users OWNER TO test;

--
-- TOC entry 238 (class 1259 OID 24784)
-- Name: chu_de; Type: TABLE; Schema: content; Owner: test
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


ALTER TABLE content.chu_de OWNER TO test;

--
-- TOC entry 239 (class 1259 OID 24794)
-- Name: lesson_plans; Type: TABLE; Schema: content; Owner: test
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
    CONSTRAINT lesson_plans_status_check CHECK (((status)::text = ANY (ARRAY[('DRAFT'::character varying)::text, ('COMPLETED'::character varying)::text, ('PUBLISHED'::character varying)::text, ('ARCHIVED'::character varying)::text])))
);


ALTER TABLE content.lesson_plans OWNER TO test;

--
-- TOC entry 240 (class 1259 OID 24806)
-- Name: lesson_templates; Type: TABLE; Schema: content; Owner: test
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
    CONSTRAINT lesson_templates_status_check CHECK (((status)::text = ANY (ARRAY[('ACTIVE'::character varying)::text, ('INACTIVE'::character varying)::text, ('ARCHIVED'::character varying)::text])))
);


ALTER TABLE content.lesson_templates OWNER TO test;

--
-- TOC entry 241 (class 1259 OID 24818)
-- Name: file_metadata; Type: TABLE; Schema: files; Owner: test
--

CREATE TABLE files.file_metadata (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    file_id uuid NOT NULL,
    key character varying(100) NOT NULL,
    value text,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE files.file_metadata OWNER TO test;

--
-- TOC entry 242 (class 1259 OID 24825)
-- Name: file_storage; Type: TABLE; Schema: files; Owner: test
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
    CONSTRAINT file_storage_file_type_check CHECK (((file_type)::text = ANY (ARRAY[('IMAGE'::character varying)::text, ('DOCUMENT'::character varying)::text, ('PDF'::character varying)::text, ('EXCEL'::character varying)::text, ('OTHER'::character varying)::text]))),
    CONSTRAINT file_storage_status_check CHECK (((status)::text = ANY (ARRAY[('ACTIVE'::character varying)::text, ('ARCHIVED'::character varying)::text, ('DELETED'::character varying)::text])))
);


ALTER TABLE files.file_storage OWNER TO test;

--
-- TOC entry 243 (class 1259 OID 24836)
-- Name: performance_metrics; Type: TABLE; Schema: logging; Owner: test
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


ALTER TABLE logging.performance_metrics OWNER TO test;

--
-- TOC entry 244 (class 1259 OID 24841)
-- Name: system_logs; Type: TABLE; Schema: logging; Owner: test
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
    CONSTRAINT system_logs_level_check CHECK (((level)::text = ANY (ARRAY[('DEBUG'::character varying)::text, ('INFO'::character varying)::text, ('WARNING'::character varying)::text, ('ERROR'::character varying)::text, ('CRITICAL'::character varying)::text])))
);


ALTER TABLE logging.system_logs OWNER TO test;

--
-- TOC entry 245 (class 1259 OID 24849)
-- Name: email_queue; Type: TABLE; Schema: notifications; Owner: test
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
    CONSTRAINT email_queue_status_check CHECK (((status)::text = ANY (ARRAY[('PENDING'::character varying)::text, ('SENT'::character varying)::text, ('FAILED'::character varying)::text])))
);


ALTER TABLE notifications.email_queue OWNER TO test;

--
-- TOC entry 246 (class 1259 OID 24859)
-- Name: notifications; Type: TABLE; Schema: notifications; Owner: test
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
    CONSTRAINT notifications_type_check CHECK (((type)::text = ANY (ARRAY[('INFO'::character varying)::text, ('SUCCESS'::character varying)::text, ('WARNING'::character varying)::text, ('ERROR'::character varying)::text])))
);


ALTER TABLE notifications.notifications OWNER TO test;

--
-- TOC entry 247 (class 1259 OID 24869)
-- Name: answer_sheets; Type: TABLE; Schema: students; Owner: test
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
    CONSTRAINT answer_sheets_ocr_status_check CHECK (((ocr_status)::text = ANY (ARRAY[('PENDING'::character varying)::text, ('PROCESSING'::character varying)::text, ('COMPLETED'::character varying)::text, ('FAILED'::character varying)::text])))
);


ALTER TABLE students.answer_sheets OWNER TO test;

--
-- TOC entry 248 (class 1259 OID 24878)
-- Name: classes; Type: TABLE; Schema: students; Owner: test
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


ALTER TABLE students.classes OWNER TO test;

--
-- TOC entry 249 (class 1259 OID 24888)
-- Name: student_results; Type: TABLE; Schema: students; Owner: test
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
    CONSTRAINT student_results_grading_method_check CHECK (((grading_method)::text = ANY (ARRAY[('OCR'::character varying)::text, ('MANUAL'::character varying)::text, ('AUTO'::character varying)::text]))),
    CONSTRAINT student_results_score_check CHECK ((score >= (0)::numeric))
);


ALTER TABLE students.student_results OWNER TO test;

--
-- TOC entry 250 (class 1259 OID 24898)
-- Name: students; Type: TABLE; Schema: students; Owner: test
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
    CONSTRAINT students_gender_check CHECK (((gender)::text = ANY (ARRAY[('MALE'::character varying)::text, ('FEMALE'::character varying)::text, ('OTHER'::character varying)::text])))
);


ALTER TABLE students.students OWNER TO test;

--
-- TOC entry 251 (class 1259 OID 24906)
-- Name: activity_logs; Type: TABLE; Schema: users; Owner: test
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


ALTER TABLE users.activity_logs OWNER TO test;

--
-- TOC entry 252 (class 1259 OID 24913)
-- Name: otp_codes; Type: TABLE; Schema: users; Owner: test
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
    CONSTRAINT otp_codes_purpose_check CHECK (((purpose)::text = ANY (ARRAY[('PASSWORD_RESET'::character varying)::text, ('EMAIL_VERIFICATION'::character varying)::text, ('PHONE_VERIFICATION'::character varying)::text])))
);


ALTER TABLE users.otp_codes OWNER TO test;

--
-- TOC entry 253 (class 1259 OID 24922)
-- Name: password_history; Type: TABLE; Schema: users; Owner: test
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


ALTER TABLE users.password_history OWNER TO test;

--
-- TOC entry 254 (class 1259 OID 24929)
-- Name: user_profiles; Type: TABLE; Schema: users; Owner: test
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


ALTER TABLE users.user_profiles OWNER TO test;

--
-- TOC entry 255 (class 1259 OID 24937)
-- Name: user_sessions; Type: TABLE; Schema: users; Owner: test
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


ALTER TABLE users.user_sessions OWNER TO test;

--
-- TOC entry 3392 (class 2604 OID 24946)
-- Name: roles id; Type: DEFAULT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.roles ALTER COLUMN id SET DEFAULT nextval('auth.roles_id_seq'::regclass);


--
-- TOC entry 3813 (class 0 OID 24690)
-- Dependencies: 226
-- Data for Name: exam_questions; Type: TABLE DATA; Schema: assessment; Owner: test
--



--
-- TOC entry 3814 (class 0 OID 24696)
-- Dependencies: 227
-- Data for Name: exams; Type: TABLE DATA; Schema: assessment; Owner: test
--



--
-- TOC entry 3815 (class 0 OID 24710)
-- Dependencies: 228
-- Data for Name: question_choices; Type: TABLE DATA; Schema: assessment; Owner: test
--

INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('ab099e94-7903-48c9-a755-f7ca1c7bcee0', 'c7495ba4-a288-42a6-bc5c-78163c388de8', 'A', 'Hạt nhân và electron', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('f7f4da56-d7b0-446b-b13a-cdda8b4414ea', 'c7495ba4-a288-42a6-bc5c-78163c388de8', 'B', 'Chỉ có hạt nhân', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('eea6a267-ef11-4a79-ac4c-f7d4aa585938', 'c7495ba4-a288-42a6-bc5c-78163c388de8', 'C', 'Chỉ có electron', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('5cb6ca66-85c0-4f1a-bc43-d64b4548fc86', 'c7495ba4-a288-42a6-bc5c-78163c388de8', 'D', 'Proton và neutron', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('70675195-aecf-487f-8910-ba3f3d6ec7d0', 'd2d42623-0b35-442e-841b-43e8f487f060', 'A', 'Theo khối lượng nguyên tử', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('bd676918-fd1d-4841-b128-2588f8fff8ad', 'd2d42623-0b35-442e-841b-43e8f487f060', 'B', 'Theo số hiệu nguyên tử tăng dần', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('fe013133-0e68-450d-ac0a-8764bc517995', 'd2d42623-0b35-442e-841b-43e8f487f060', 'C', 'Theo tên nguyên tố', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('c44d0128-a206-4de5-9d8b-c218c6d5be72', 'd2d42623-0b35-442e-841b-43e8f487f060', 'D', 'Theo màu sắc', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('022a8315-256e-4b77-8d90-f7cd73fecb89', 'a77aa669-4623-4104-99d6-684673a578c9', 'C', 'HNO3', '2025-09-12 08:12:05.860273');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('62578e12-bfe2-4052-8a29-4916dede6152', 'a77aa669-4623-4104-99d6-684673a578c9', 'D', 'H3PO4', '2025-09-12 08:12:05.860273');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('d6f365ef-0b8d-4ba9-8cc4-3a1afa1c40c9', 'a77aa669-4623-4104-99d6-684673a578c9', 'B', 'H2SO4', '2025-09-12 08:12:05.860273');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('fd78c35a-a2d1-4c36-892d-de5a9eaa0fe3', 'a77aa669-4623-4104-99d6-684673a578c9', 'A', 'HCl', '2025-09-12 08:12:05.860205');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('19679690-3184-4410-abb0-d19d71687071', '7f314536-f6f0-41ec-9e88-2c5671897362', 'D', 'H3PO4', '2025-09-12 08:12:46.595286');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('502d6a2b-b1c2-434c-8430-7a444ededfe7', '7f314536-f6f0-41ec-9e88-2c5671897362', 'B', 'H2SO4', '2025-09-12 08:12:46.595286');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('5d39a298-6ca9-4ebf-87b1-4f4aa7308e8b', '7f314536-f6f0-41ec-9e88-2c5671897362', 'C', 'HNO3', '2025-09-12 08:12:46.595286');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('76a8d71f-934f-461f-b638-4fab6965fcaa', '7f314536-f6f0-41ec-9e88-2c5671897362', 'A', 'HCl', '2025-09-12 08:12:46.595284');


--
-- TOC entry 3816 (class 0 OID 24718)
-- Dependencies: 229
-- Data for Name: questions; Type: TABLE DATA; Schema: assessment; Owner: test
--

INSERT INTO assessment.questions (id, content, type, difficulty, subject, topic, correct_answer, explanation, created_by, status, created_at, updated_at, points) VALUES ('c7495ba4-a288-42a6-bc5c-78163c388de8', 'Nguyên tử có cấu trúc như thế nào?', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 'Cấu trúc nguyên tử', 'A', 'Nguyên tử gồm hạt nhân và electron', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764', 1.00);
INSERT INTO assessment.questions (id, content, type, difficulty, subject, topic, correct_answer, explanation, created_by, status, created_at, updated_at, points) VALUES ('d2d42623-0b35-442e-841b-43e8f487f060', 'Trong bảng tuần hoàn, các nguyên tố được sắp xếp theo:', 'MULTIPLE_CHOICE', 'MEDIUM', 'HOA_HOC', 'Bảng tuần hoàn', 'B', 'Theo số hiệu nguyên tử tăng dần', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764', 1.00);
INSERT INTO assessment.questions (id, content, type, difficulty, subject, topic, correct_answer, explanation, created_by, status, created_at, updated_at, points) VALUES ('6ebffe14-1599-4ebe-894e-87241d5caeac', 'Test question', 'MULTIPLE_CHOICE', 'MEDIUM', 'HOA_HOC', NULL, NULL, NULL, '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-12 08:03:51.987265', '2025-09-12 08:03:51.987265', 1.00);
INSERT INTO assessment.questions (id, content, type, difficulty, subject, topic, correct_answer, explanation, created_by, status, created_at, updated_at, points) VALUES ('a77aa669-4623-4104-99d6-684673a578c9', 'Hãy chọn công thức hóa học đúng của axit clohiđric:', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 'Axit - Bazơ - Muối', 'A', 'Axit clohiđric có công thức HCl', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-12 08:12:05.859811', '2025-09-12 08:12:05.859812', 1.00);
INSERT INTO assessment.questions (id, content, type, difficulty, subject, topic, correct_answer, explanation, created_by, status, created_at, updated_at, points) VALUES ('7f314536-f6f0-41ec-9e88-2c5671897362', 'Hãy chọn công thức hóa học đúng của axit clohiđric:', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 'Axit - Bazơ - Muối', 'A', 'Axit clohiđric có công thức HCl', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-12 08:12:46.595278', '2025-09-12 08:12:46.595278', 1.00);


--
-- TOC entry 3817 (class 0 OID 24731)
-- Dependencies: 230
-- Data for Name: email_verifications; Type: TABLE DATA; Schema: auth; Owner: test
--



--
-- TOC entry 3818 (class 0 OID 24737)
-- Dependencies: 231
-- Data for Name: password_history; Type: TABLE DATA; Schema: auth; Owner: test
--



--
-- TOC entry 3819 (class 0 OID 24742)
-- Dependencies: 232
-- Data for Name: password_resets; Type: TABLE DATA; Schema: auth; Owner: test
--



--
-- TOC entry 3820 (class 0 OID 24749)
-- Dependencies: 233
-- Data for Name: roles; Type: TABLE DATA; Schema: auth; Owner: test
--

INSERT INTO auth.roles (id, name, description, is_active, created_at, updated_at) VALUES (1, 'ADMIN', 'Quản trị viên hệ thống', true, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO auth.roles (id, name, description, is_active, created_at, updated_at) VALUES (2, 'MANAGER', 'Quản lý nội dung và người dùng', true, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO auth.roles (id, name, description, is_active, created_at, updated_at) VALUES (3, 'STAFF', 'Nhân viên tạo nội dung mẫu', true, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO auth.roles (id, name, description, is_active, created_at, updated_at) VALUES (4, 'TEACHER', 'Giáo viên sử dụng hệ thống', true, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');


--
-- TOC entry 3822 (class 0 OID 24758)
-- Dependencies: 235
-- Data for Name: sessions; Type: TABLE DATA; Schema: auth; Owner: test
--

INSERT INTO auth.sessions (id, user_id, token, expires_at, created_at) VALUES ('67468aac-4895-4760-9ba9-b9734706de3e', '550e8400-e29b-41d4-a716-446655440002', 'P4mFhLeEK/HHWgKSJCgYg86p+PJz5bNtlVFTZB8uwgsuZdWWVmuNDXOQNquqvEtUSw8z0Pk30xjC8tU46ydXkw==', '2025-09-19 07:41:25.429243', '2025-09-12 07:41:25.429186');
INSERT INTO auth.sessions (id, user_id, token, expires_at, created_at) VALUES ('bf4c06e6-4b4d-4fec-b8c8-90cef0331543', '550e8400-e29b-41d4-a716-446655440002', 'HQKszHg4l+OUReqe2143zFaGBMthiIf5TPGgseViDrr5bdOkUxfD7UwvtQZ/UqBaGQlQXb/NGMS5HFuIf4w+5g==', '2025-09-19 07:41:56.986942', '2025-09-12 07:41:56.986942');
INSERT INTO auth.sessions (id, user_id, token, expires_at, created_at) VALUES ('a89d818e-72fe-4ae7-8073-0fa8e9a2a4ab', '550e8400-e29b-41d4-a716-446655440002', '7b49McChO7wX/cgXy7W9kvCBKJJwja45SruPvVyOzqwrkIxGzsmW7ft/eJtmwHPXC2QmePpGJikMA6svaeJ19w==', '2025-09-19 07:58:38.030585', '2025-09-12 07:58:38.030526');
INSERT INTO auth.sessions (id, user_id, token, expires_at, created_at) VALUES ('51bbc143-637c-4145-afce-8ddb4be39b75', '550e8400-e29b-41d4-a716-446655440002', 'jWd9BG7oRqDa9GVQNCTxeFbCaClqPvYIl+EcRe8i/RNezWxB+gR7QfdzSywev1RoY0j/4eAPBg1WSk9rzuCRjg==', '2025-09-19 08:05:44.24244', '2025-09-12 08:05:44.242344');
INSERT INTO auth.sessions (id, user_id, token, expires_at, created_at) VALUES ('390d157a-a0af-43db-b29f-3805e0172e0c', '550e8400-e29b-41d4-a716-446655440002', '1dOIqpdKj24Fhm2dG6NLsIbCCzp/u1FtOr93qkrC1uQHIPKE/ma7//kqIs+LllTlVUgqeWg3mRcvjnOCkdjeyw==', '2025-09-19 08:08:41.474685', '2025-09-12 08:08:41.474684');
INSERT INTO auth.sessions (id, user_id, token, expires_at, created_at) VALUES ('fcdf5bd1-b0fe-49ec-887d-d7b184571215', '550e8400-e29b-41d4-a716-446655440002', '0HyJJpiLlkmC/pj5xMC1r2mHUE9keMkYUyA40DL8YU2Vgn/ZAY+a6sBrTjGngl0xdfLtwTpktR/dIB/Hh9Q+vw==', '2025-09-19 08:12:26.569055', '2025-09-12 08:12:26.568987');
INSERT INTO auth.sessions (id, user_id, token, expires_at, created_at) VALUES ('1a537796-fb7c-4533-b2b8-e073c9cda416', '550e8400-e29b-41d4-a716-446655440002', 'z7u3yIzs5zPr96ZneiOgRWbKYMmdwT/9Eze8hxwFisWCOWsOO9JtCa+sXnZsIF8IoOfBiH/WRh9I2hKylZDTfw==', '2025-09-19 10:51:54.232557', '2025-09-12 10:51:54.232309');


--
-- TOC entry 3823 (class 0 OID 24765)
-- Dependencies: 236
-- Data for Name: user_sessions; Type: TABLE DATA; Schema: auth; Owner: test
--



--
-- TOC entry 3824 (class 0 OID 24774)
-- Dependencies: 237
-- Data for Name: users; Type: TABLE DATA; Schema: auth; Owner: test
--

INSERT INTO auth.users (id, email, password_hash, role_id, is_active, last_login, created_at, updated_at, deleted_at, is_deleted) VALUES ('550e8400-e29b-41d4-a716-446655440001', 'admin@planbookai.com', '$2a$06$TR3M6zDkfP0uviJmf4smxOZZl27bTP2xVBkRu46csEypzXjdY5rUe', 1, true, NULL, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764', NULL, false);
INSERT INTO auth.users (id, email, password_hash, role_id, is_active, last_login, created_at, updated_at, deleted_at, is_deleted) VALUES ('550e8400-e29b-41d4-a716-446655440002', 'teacher@test.com', '$2a$06$iTsgec8JGNBPUF.Bf7axTOzWhxQ9q8AZ8AIDpAsu5SDZ9KlbR6a4u', 4, true, '2025-09-12 10:51:53.895923', '2025-09-12 07:40:16.04764', '2025-09-12 10:51:54.024551', NULL, false);


--
-- TOC entry 3825 (class 0 OID 24784)
-- Dependencies: 238
-- Data for Name: chu_de; Type: TABLE DATA; Schema: content; Owner: test
--

INSERT INTO content.chu_de (id, name, description, subject, grade, parent_id, created_by, created_at, updated_at) VALUES ('bece3563-0ee0-4623-96d3-57e1af7ea39f', 'Cấu trúc nguyên tử', 'Nghiên cứu về cấu trúc và tính chất của nguyên tử', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO content.chu_de (id, name, description, subject, grade, parent_id, created_by, created_at, updated_at) VALUES ('f109f1be-3eb1-42bc-a1a3-adbf723b1875', 'Bảng tuần hoàn', 'Nghiên cứu về bảng tuần hoàn các nguyên tố hóa học', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO content.chu_de (id, name, description, subject, grade, parent_id, created_by, created_at, updated_at) VALUES ('4ff055b6-f594-4225-881f-8111ca09af53', 'Liên kết hóa học', 'Nghiên cứu về các loại liên kết hóa học', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');


--
-- TOC entry 3826 (class 0 OID 24794)
-- Dependencies: 239
-- Data for Name: lesson_plans; Type: TABLE DATA; Schema: content; Owner: test
--



--
-- TOC entry 3827 (class 0 OID 24806)
-- Dependencies: 240
-- Data for Name: lesson_templates; Type: TABLE DATA; Schema: content; Owner: test
--



--
-- TOC entry 3828 (class 0 OID 24818)
-- Dependencies: 241
-- Data for Name: file_metadata; Type: TABLE DATA; Schema: files; Owner: test
--



--
-- TOC entry 3829 (class 0 OID 24825)
-- Dependencies: 242
-- Data for Name: file_storage; Type: TABLE DATA; Schema: files; Owner: test
--



--
-- TOC entry 3830 (class 0 OID 24836)
-- Dependencies: 243
-- Data for Name: performance_metrics; Type: TABLE DATA; Schema: logging; Owner: test
--



--
-- TOC entry 3831 (class 0 OID 24841)
-- Dependencies: 244
-- Data for Name: system_logs; Type: TABLE DATA; Schema: logging; Owner: test
--



--
-- TOC entry 3832 (class 0 OID 24849)
-- Dependencies: 245
-- Data for Name: email_queue; Type: TABLE DATA; Schema: notifications; Owner: test
--



--
-- TOC entry 3833 (class 0 OID 24859)
-- Dependencies: 246
-- Data for Name: notifications; Type: TABLE DATA; Schema: notifications; Owner: test
--



--
-- TOC entry 3834 (class 0 OID 24869)
-- Dependencies: 247
-- Data for Name: answer_sheets; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3835 (class 0 OID 24878)
-- Dependencies: 248
-- Data for Name: classes; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3836 (class 0 OID 24888)
-- Dependencies: 249
-- Data for Name: student_results; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3837 (class 0 OID 24898)
-- Dependencies: 250
-- Data for Name: students; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3838 (class 0 OID 24906)
-- Dependencies: 251
-- Data for Name: activity_logs; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3839 (class 0 OID 24913)
-- Dependencies: 252
-- Data for Name: otp_codes; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3840 (class 0 OID 24922)
-- Dependencies: 253
-- Data for Name: password_history; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3841 (class 0 OID 24929)
-- Dependencies: 254
-- Data for Name: user_profiles; Type: TABLE DATA; Schema: users; Owner: test
--

INSERT INTO users.user_profiles (id, user_id, full_name, phone, address, bio, avatar_url, created_at, updated_at) VALUES ('87209c76-3e2a-4538-b3ed-48ac22a18913', '550e8400-e29b-41d4-a716-446655440001', 'Admin System', '0123456789', 'Hà Nội', 'Quản trị viên hệ thống', NULL, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO users.user_profiles (id, user_id, full_name, phone, address, bio, avatar_url, created_at, updated_at) VALUES ('8889c7bc-2751-47ed-ac61-24ba3cf3c0d5', '550e8400-e29b-41d4-a716-446655440002', 'Giáo viên Test', '0987654321', 'TP.HCM', 'Giáo viên Hóa học THPT', NULL, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');


--
-- TOC entry 3842 (class 0 OID 24937)
-- Dependencies: 255
-- Data for Name: user_sessions; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3851 (class 0 OID 0)
-- Dependencies: 234
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: auth; Owner: test
--

SELECT pg_catalog.setval('auth.roles_id_seq', 1, false);


--
-- TOC entry 3497 (class 2606 OID 24948)
-- Name: exam_questions exam_questions_exam_id_question_id_key; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_question_id_key UNIQUE (exam_id, question_id);


--
-- TOC entry 3499 (class 2606 OID 24950)
-- Name: exam_questions exam_questions_exam_id_question_order_key; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_question_order_key UNIQUE (exam_id, question_order);


--
-- TOC entry 3501 (class 2606 OID 24952)
-- Name: exam_questions exam_questions_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_pkey PRIMARY KEY (id);


--
-- TOC entry 3503 (class 2606 OID 24954)
-- Name: exams exams_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_pkey PRIMARY KEY (id);


--
-- TOC entry 3508 (class 2606 OID 24956)
-- Name: question_choices question_choices_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_pkey PRIMARY KEY (id);


--
-- TOC entry 3510 (class 2606 OID 24958)
-- Name: question_choices question_choices_question_id_choice_order_key; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_question_id_choice_order_key UNIQUE (question_id, choice_order);


--
-- TOC entry 3516 (class 2606 OID 24960)
-- Name: questions questions_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.questions
    ADD CONSTRAINT questions_pkey PRIMARY KEY (id);


--
-- TOC entry 3518 (class 2606 OID 24962)
-- Name: email_verifications email_verifications_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_pkey PRIMARY KEY (id);


--
-- TOC entry 3520 (class 2606 OID 24964)
-- Name: email_verifications email_verifications_verification_token_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_verification_token_key UNIQUE (verification_token);


--
-- TOC entry 3522 (class 2606 OID 24966)
-- Name: password_history password_history_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.password_history
    ADD CONSTRAINT password_history_pkey PRIMARY KEY (id);


--
-- TOC entry 3524 (class 2606 OID 24968)
-- Name: password_resets password_resets_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.password_resets
    ADD CONSTRAINT password_resets_pkey PRIMARY KEY (id);


--
-- TOC entry 3526 (class 2606 OID 24970)
-- Name: roles roles_name_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.roles
    ADD CONSTRAINT roles_name_key UNIQUE (name);


--
-- TOC entry 3528 (class 2606 OID 24972)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- TOC entry 3530 (class 2606 OID 24974)
-- Name: sessions sessions_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3532 (class 2606 OID 24976)
-- Name: sessions sessions_token_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_token_key UNIQUE (token);


--
-- TOC entry 3534 (class 2606 OID 24978)
-- Name: user_sessions user_sessions_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3536 (class 2606 OID 24980)
-- Name: user_sessions user_sessions_session_token_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_session_token_key UNIQUE (session_token);


--
-- TOC entry 3538 (class 2606 OID 24982)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 3540 (class 2606 OID 24984)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 3542 (class 2606 OID 24986)
-- Name: chu_de chu_de_pkey; Type: CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_pkey PRIMARY KEY (id);


--
-- TOC entry 3549 (class 2606 OID 24988)
-- Name: lesson_plans lesson_plans_pkey; Type: CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_pkey PRIMARY KEY (id);


--
-- TOC entry 3552 (class 2606 OID 24990)
-- Name: lesson_templates lesson_templates_pkey; Type: CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_templates
    ADD CONSTRAINT lesson_templates_pkey PRIMARY KEY (id);


--
-- TOC entry 3554 (class 2606 OID 24992)
-- Name: file_metadata file_metadata_file_id_key_key; Type: CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_file_id_key_key UNIQUE (file_id, key);


--
-- TOC entry 3556 (class 2606 OID 24994)
-- Name: file_metadata file_metadata_pkey; Type: CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_pkey PRIMARY KEY (id);


--
-- TOC entry 3559 (class 2606 OID 24996)
-- Name: file_storage file_storage_pkey; Type: CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_storage
    ADD CONSTRAINT file_storage_pkey PRIMARY KEY (id);


--
-- TOC entry 3566 (class 2606 OID 24998)
-- Name: performance_metrics performance_metrics_pkey; Type: CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.performance_metrics
    ADD CONSTRAINT performance_metrics_pkey PRIMARY KEY (id);


--
-- TOC entry 3570 (class 2606 OID 25000)
-- Name: system_logs system_logs_pkey; Type: CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.system_logs
    ADD CONSTRAINT system_logs_pkey PRIMARY KEY (id);


--
-- TOC entry 3572 (class 2606 OID 25002)
-- Name: email_queue email_queue_pkey; Type: CONSTRAINT; Schema: notifications; Owner: test
--

ALTER TABLE ONLY notifications.email_queue
    ADD CONSTRAINT email_queue_pkey PRIMARY KEY (id);


--
-- TOC entry 3578 (class 2606 OID 25004)
-- Name: notifications notifications_pkey; Type: CONSTRAINT; Schema: notifications; Owner: test
--

ALTER TABLE ONLY notifications.notifications
    ADD CONSTRAINT notifications_pkey PRIMARY KEY (id);


--
-- TOC entry 3580 (class 2606 OID 25006)
-- Name: answer_sheets answer_sheets_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_pkey PRIMARY KEY (id);


--
-- TOC entry 3584 (class 2606 OID 25008)
-- Name: classes classes_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.classes
    ADD CONSTRAINT classes_pkey PRIMARY KEY (id);


--
-- TOC entry 3589 (class 2606 OID 25010)
-- Name: student_results student_results_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_pkey PRIMARY KEY (id);


--
-- TOC entry 3591 (class 2606 OID 25012)
-- Name: student_results student_results_student_id_exam_id_key; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_student_id_exam_id_key UNIQUE (student_id, exam_id);


--
-- TOC entry 3596 (class 2606 OID 25014)
-- Name: students students_owner_teacher_id_student_code_key; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_owner_teacher_id_student_code_key UNIQUE (owner_teacher_id, student_code);


--
-- TOC entry 3598 (class 2606 OID 25016)
-- Name: students students_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_pkey PRIMARY KEY (id);


--
-- TOC entry 3600 (class 2606 OID 25018)
-- Name: activity_logs activity_logs_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.activity_logs
    ADD CONSTRAINT activity_logs_pkey PRIMARY KEY (id);


--
-- TOC entry 3607 (class 2606 OID 25020)
-- Name: otp_codes otp_codes_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.otp_codes
    ADD CONSTRAINT otp_codes_pkey PRIMARY KEY (id);


--
-- TOC entry 3610 (class 2606 OID 25022)
-- Name: password_history password_history_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_pkey PRIMARY KEY (id);


--
-- TOC entry 3613 (class 2606 OID 25024)
-- Name: user_profiles user_profiles_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_profiles
    ADD CONSTRAINT user_profiles_pkey PRIMARY KEY (id);


--
-- TOC entry 3618 (class 2606 OID 25026)
-- Name: user_sessions user_sessions_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3620 (class 2606 OID 25028)
-- Name: user_sessions user_sessions_session_token_key; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_session_token_key UNIQUE (session_token);


--
-- TOC entry 3504 (class 1259 OID 25029)
-- Name: idx_assessment_exams_subject; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_exams_subject ON assessment.exams USING btree (subject);


--
-- TOC entry 3505 (class 1259 OID 25030)
-- Name: idx_assessment_exams_teacher; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_exams_teacher ON assessment.exams USING btree (teacher_id);


--
-- TOC entry 3506 (class 1259 OID 25031)
-- Name: idx_assessment_question_choices_question; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_question_choices_question ON assessment.question_choices USING btree (question_id);


--
-- TOC entry 3511 (class 1259 OID 25032)
-- Name: idx_assessment_questions_created_by; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_created_by ON assessment.questions USING btree (created_by);


--
-- TOC entry 3512 (class 1259 OID 25033)
-- Name: idx_assessment_questions_difficulty; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_difficulty ON assessment.questions USING btree (difficulty);


--
-- TOC entry 3513 (class 1259 OID 25034)
-- Name: idx_assessment_questions_subject; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_subject ON assessment.questions USING btree (subject);


--
-- TOC entry 3514 (class 1259 OID 25035)
-- Name: idx_assessment_questions_topic; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_topic ON assessment.questions USING btree (topic);


--
-- TOC entry 3543 (class 1259 OID 25036)
-- Name: idx_content_chu_de_parent; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_chu_de_parent ON content.chu_de USING btree (parent_id);


--
-- TOC entry 3544 (class 1259 OID 25037)
-- Name: idx_content_chu_de_subject; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_chu_de_subject ON content.chu_de USING btree (subject);


--
-- TOC entry 3545 (class 1259 OID 25038)
-- Name: idx_content_lesson_plans_subject; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_lesson_plans_subject ON content.lesson_plans USING btree (subject);


--
-- TOC entry 3546 (class 1259 OID 25039)
-- Name: idx_content_lesson_plans_teacher; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_lesson_plans_teacher ON content.lesson_plans USING btree (teacher_id);


--
-- TOC entry 3550 (class 1259 OID 25040)
-- Name: idx_content_lesson_templates_subject; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_lesson_templates_subject ON content.lesson_templates USING btree (subject);


--
-- TOC entry 3547 (class 1259 OID 25041)
-- Name: idx_lesson_plans_topic; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_lesson_plans_topic ON content.lesson_plans USING btree (topic_id);


--
-- TOC entry 3557 (class 1259 OID 25042)
-- Name: idx_files_metadata_file; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_metadata_file ON files.file_metadata USING btree (file_id);


--
-- TOC entry 3560 (class 1259 OID 25043)
-- Name: idx_files_storage_file_type; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_storage_file_type ON files.file_storage USING btree (file_type);


--
-- TOC entry 3561 (class 1259 OID 25044)
-- Name: idx_files_storage_status; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_storage_status ON files.file_storage USING btree (status);


--
-- TOC entry 3562 (class 1259 OID 25045)
-- Name: idx_files_storage_uploaded_by; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_storage_uploaded_by ON files.file_storage USING btree (uploaded_by);


--
-- TOC entry 3563 (class 1259 OID 25046)
-- Name: idx_logging_performance_created_at; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_performance_created_at ON logging.performance_metrics USING btree (created_at);


--
-- TOC entry 3564 (class 1259 OID 25047)
-- Name: idx_logging_performance_service; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_performance_service ON logging.performance_metrics USING btree (service_name);


--
-- TOC entry 3567 (class 1259 OID 25048)
-- Name: idx_logging_system_logs_created_at; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_system_logs_created_at ON logging.system_logs USING btree (created_at);


--
-- TOC entry 3568 (class 1259 OID 25049)
-- Name: idx_logging_system_logs_level; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_system_logs_level ON logging.system_logs USING btree (level);


--
-- TOC entry 3573 (class 1259 OID 25050)
-- Name: idx_email_queue_status; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_email_queue_status ON notifications.email_queue USING btree (status);


--
-- TOC entry 3574 (class 1259 OID 25051)
-- Name: idx_notifications_created_at; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_notifications_created_at ON notifications.notifications USING btree (created_at);


--
-- TOC entry 3575 (class 1259 OID 25052)
-- Name: idx_notifications_is_read; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_notifications_is_read ON notifications.notifications USING btree (is_read);


--
-- TOC entry 3576 (class 1259 OID 25053)
-- Name: idx_notifications_user; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_notifications_user ON notifications.notifications USING btree (user_id);


--
-- TOC entry 3581 (class 1259 OID 25054)
-- Name: idx_students_answer_sheets_exam; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_answer_sheets_exam ON students.answer_sheets USING btree (exam_id);


--
-- TOC entry 3582 (class 1259 OID 25055)
-- Name: idx_students_answer_sheets_student; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_answer_sheets_student ON students.answer_sheets USING btree (student_id);


--
-- TOC entry 3585 (class 1259 OID 25056)
-- Name: idx_students_classes_teacher; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_classes_teacher ON students.classes USING btree (homeroom_teacher_id);


--
-- TOC entry 3586 (class 1259 OID 25057)
-- Name: idx_students_student_results_exam; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_student_results_exam ON students.student_results USING btree (exam_id);


--
-- TOC entry 3587 (class 1259 OID 25058)
-- Name: idx_students_student_results_student; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_student_results_student ON students.student_results USING btree (student_id);


--
-- TOC entry 3592 (class 1259 OID 25059)
-- Name: idx_students_students_class; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_students_class ON students.students USING btree (class_id);


--
-- TOC entry 3593 (class 1259 OID 25060)
-- Name: idx_students_students_code; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_students_code ON students.students USING btree (student_code);


--
-- TOC entry 3594 (class 1259 OID 25061)
-- Name: idx_students_students_teacher; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_students_teacher ON students.students USING btree (owner_teacher_id);


--
-- TOC entry 3601 (class 1259 OID 25062)
-- Name: idx_users_activity_logs_created; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_activity_logs_created ON users.activity_logs USING btree (created_at);


--
-- TOC entry 3602 (class 1259 OID 25063)
-- Name: idx_users_activity_logs_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_activity_logs_user ON users.activity_logs USING btree (user_id);


--
-- TOC entry 3603 (class 1259 OID 25064)
-- Name: idx_users_otp_codes_expires; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_otp_codes_expires ON users.otp_codes USING btree (expires_at);


--
-- TOC entry 3604 (class 1259 OID 25065)
-- Name: idx_users_otp_codes_purpose; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_otp_codes_purpose ON users.otp_codes USING btree (purpose);


--
-- TOC entry 3605 (class 1259 OID 25066)
-- Name: idx_users_otp_codes_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_otp_codes_user ON users.otp_codes USING btree (user_id);


--
-- TOC entry 3608 (class 1259 OID 25067)
-- Name: idx_users_password_history_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_password_history_user ON users.password_history USING btree (user_id);


--
-- TOC entry 3611 (class 1259 OID 25068)
-- Name: idx_users_profiles_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_profiles_user ON users.user_profiles USING btree (user_id);


--
-- TOC entry 3614 (class 1259 OID 25069)
-- Name: idx_users_user_sessions_active; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_user_sessions_active ON users.user_sessions USING btree (is_active);


--
-- TOC entry 3615 (class 1259 OID 25070)
-- Name: idx_users_user_sessions_token; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_user_sessions_token ON users.user_sessions USING btree (session_token);


--
-- TOC entry 3616 (class 1259 OID 25071)
-- Name: idx_users_user_sessions_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_user_sessions_user ON users.user_sessions USING btree (user_id);


--
-- TOC entry 3656 (class 2620 OID 25072)
-- Name: exams trigger_updated_at_assessment_exams; Type: TRIGGER; Schema: assessment; Owner: test
--

CREATE TRIGGER trigger_updated_at_assessment_exams BEFORE UPDATE ON assessment.exams FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3657 (class 2620 OID 25073)
-- Name: questions trigger_updated_at_assessment_questions; Type: TRIGGER; Schema: assessment; Owner: test
--

CREATE TRIGGER trigger_updated_at_assessment_questions BEFORE UPDATE ON assessment.questions FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3658 (class 2620 OID 25074)
-- Name: roles trigger_updated_at_auth_roles; Type: TRIGGER; Schema: auth; Owner: test
--

CREATE TRIGGER trigger_updated_at_auth_roles BEFORE UPDATE ON auth.roles FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3659 (class 2620 OID 25075)
-- Name: users trigger_updated_at_auth_users; Type: TRIGGER; Schema: auth; Owner: test
--

CREATE TRIGGER trigger_updated_at_auth_users BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3660 (class 2620 OID 25260)
-- Name: users trigger_updated_at_auth_users_soft_delete; Type: TRIGGER; Schema: auth; Owner: test
--

CREATE TRIGGER trigger_updated_at_auth_users_soft_delete BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3661 (class 2620 OID 25077)
-- Name: chu_de trigger_updated_at_content_chu_de; Type: TRIGGER; Schema: content; Owner: test
--

CREATE TRIGGER trigger_updated_at_content_chu_de BEFORE UPDATE ON content.chu_de FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3662 (class 2620 OID 25078)
-- Name: lesson_plans trigger_updated_at_content_lesson_plans; Type: TRIGGER; Schema: content; Owner: test
--

CREATE TRIGGER trigger_updated_at_content_lesson_plans BEFORE UPDATE ON content.lesson_plans FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3663 (class 2620 OID 25079)
-- Name: lesson_templates trigger_updated_at_content_lesson_templates; Type: TRIGGER; Schema: content; Owner: test
--

CREATE TRIGGER trigger_updated_at_content_lesson_templates BEFORE UPDATE ON content.lesson_templates FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3664 (class 2620 OID 25080)
-- Name: file_storage trigger_updated_at_files_file_storage; Type: TRIGGER; Schema: files; Owner: test
--

CREATE TRIGGER trigger_updated_at_files_file_storage BEFORE UPDATE ON files.file_storage FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3665 (class 2620 OID 25081)
-- Name: classes trigger_updated_at_students_classes; Type: TRIGGER; Schema: students; Owner: test
--

CREATE TRIGGER trigger_updated_at_students_classes BEFORE UPDATE ON students.classes FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3666 (class 2620 OID 25082)
-- Name: students trigger_updated_at_students_students; Type: TRIGGER; Schema: students; Owner: test
--

CREATE TRIGGER trigger_updated_at_students_students BEFORE UPDATE ON students.students FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3667 (class 2620 OID 25083)
-- Name: user_profiles trigger_updated_at_users_user_profiles; Type: TRIGGER; Schema: users; Owner: test
--

CREATE TRIGGER trigger_updated_at_users_user_profiles BEFORE UPDATE ON users.user_profiles FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3621 (class 2606 OID 25084)
-- Name: exam_questions exam_questions_exam_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id) ON DELETE CASCADE;


--
-- TOC entry 3622 (class 2606 OID 25089)
-- Name: exam_questions exam_questions_question_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_question_id_fkey FOREIGN KEY (question_id) REFERENCES assessment.questions(id);


--
-- TOC entry 3623 (class 2606 OID 25094)
-- Name: exams exams_teacher_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3624 (class 2606 OID 25099)
-- Name: question_choices question_choices_question_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_question_id_fkey FOREIGN KEY (question_id) REFERENCES assessment.questions(id) ON DELETE CASCADE;


--
-- TOC entry 3625 (class 2606 OID 25104)
-- Name: questions questions_created_by_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.questions
    ADD CONSTRAINT questions_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3626 (class 2606 OID 25109)
-- Name: email_verifications email_verifications_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3627 (class 2606 OID 25114)
-- Name: password_history password_history_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.password_history
    ADD CONSTRAINT password_history_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3628 (class 2606 OID 25119)
-- Name: password_resets password_resets_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.password_resets
    ADD CONSTRAINT password_resets_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3629 (class 2606 OID 25124)
-- Name: sessions sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3630 (class 2606 OID 25129)
-- Name: user_sessions user_sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3631 (class 2606 OID 25134)
-- Name: users users_role_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES auth.roles(id);


--
-- TOC entry 3632 (class 2606 OID 25139)
-- Name: chu_de chu_de_created_by_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3633 (class 2606 OID 25144)
-- Name: chu_de chu_de_parent_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_parent_id_fkey FOREIGN KEY (parent_id) REFERENCES content.chu_de(id);


--
-- TOC entry 3634 (class 2606 OID 25149)
-- Name: lesson_plans fk_lesson_plans_topic; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT fk_lesson_plans_topic FOREIGN KEY (topic_id) REFERENCES content.chu_de(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 3635 (class 2606 OID 25154)
-- Name: lesson_plans lesson_plans_teacher_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3636 (class 2606 OID 25159)
-- Name: lesson_plans lesson_plans_template_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_template_id_fkey FOREIGN KEY (template_id) REFERENCES content.lesson_templates(id);


--
-- TOC entry 3637 (class 2606 OID 25164)
-- Name: lesson_templates lesson_templates_created_by_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_templates
    ADD CONSTRAINT lesson_templates_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3638 (class 2606 OID 25169)
-- Name: file_metadata file_metadata_file_id_fkey; Type: FK CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_file_id_fkey FOREIGN KEY (file_id) REFERENCES files.file_storage(id) ON DELETE CASCADE;


--
-- TOC entry 3639 (class 2606 OID 25174)
-- Name: file_storage file_storage_uploaded_by_fkey; Type: FK CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_storage
    ADD CONSTRAINT file_storage_uploaded_by_fkey FOREIGN KEY (uploaded_by) REFERENCES auth.users(id);


--
-- TOC entry 3640 (class 2606 OID 25179)
-- Name: performance_metrics performance_metrics_user_id_fkey; Type: FK CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.performance_metrics
    ADD CONSTRAINT performance_metrics_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3641 (class 2606 OID 25184)
-- Name: system_logs system_logs_user_id_fkey; Type: FK CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.system_logs
    ADD CONSTRAINT system_logs_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3642 (class 2606 OID 25189)
-- Name: notifications notifications_user_id_fkey; Type: FK CONSTRAINT; Schema: notifications; Owner: test
--

ALTER TABLE ONLY notifications.notifications
    ADD CONSTRAINT notifications_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3643 (class 2606 OID 25194)
-- Name: answer_sheets answer_sheets_exam_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id);


--
-- TOC entry 3644 (class 2606 OID 25199)
-- Name: answer_sheets answer_sheets_student_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_student_id_fkey FOREIGN KEY (student_id) REFERENCES students.students(id);


--
-- TOC entry 3645 (class 2606 OID 25204)
-- Name: classes classes_homeroom_teacher_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.classes
    ADD CONSTRAINT classes_homeroom_teacher_id_fkey FOREIGN KEY (homeroom_teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3646 (class 2606 OID 25209)
-- Name: student_results student_results_exam_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id);


--
-- TOC entry 3647 (class 2606 OID 25214)
-- Name: student_results student_results_student_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_student_id_fkey FOREIGN KEY (student_id) REFERENCES students.students(id);


--
-- TOC entry 3648 (class 2606 OID 25219)
-- Name: students students_class_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_class_id_fkey FOREIGN KEY (class_id) REFERENCES students.classes(id);


--
-- TOC entry 3649 (class 2606 OID 25224)
-- Name: students students_owner_teacher_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_owner_teacher_id_fkey FOREIGN KEY (owner_teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3650 (class 2606 OID 25229)
-- Name: activity_logs activity_logs_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.activity_logs
    ADD CONSTRAINT activity_logs_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3651 (class 2606 OID 25234)
-- Name: otp_codes otp_codes_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.otp_codes
    ADD CONSTRAINT otp_codes_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3652 (class 2606 OID 25239)
-- Name: password_history password_history_changed_by_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_changed_by_fkey FOREIGN KEY (changed_by) REFERENCES auth.users(id);


--
-- TOC entry 3653 (class 2606 OID 25244)
-- Name: password_history password_history_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3654 (class 2606 OID 25249)
-- Name: user_profiles user_profiles_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_profiles
    ADD CONSTRAINT user_profiles_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3655 (class 2606 OID 25254)
-- Name: user_sessions user_sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


-- Completed on 2025-09-12 21:34:56

--
-- PostgreSQL database dump complete
--

