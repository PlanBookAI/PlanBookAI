--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5 (Debian 17.5-1.pgdg120+1)
-- Dumped by pg_dump version 17.5

-- Started on 2025-08-31 23:12:58

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

DROP DATABASE planbookai;
--
-- TOC entry 3824 (class 1262 OID 16388)
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
-- TOC entry 9 (class 2615 OID 16391)
-- Name: assessment; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA assessment;


ALTER SCHEMA assessment OWNER TO test;

--
-- TOC entry 7 (class 2615 OID 16389)
-- Name: auth; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA auth;


ALTER SCHEMA auth OWNER TO test;

--
-- TOC entry 10 (class 2615 OID 16392)
-- Name: content; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA content;


ALTER SCHEMA content OWNER TO test;

--
-- TOC entry 12 (class 2615 OID 16394)
-- Name: files; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA files;


ALTER SCHEMA files OWNER TO test;

--
-- TOC entry 14 (class 2615 OID 16396)
-- Name: logging; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA logging;


ALTER SCHEMA logging OWNER TO test;

--
-- TOC entry 13 (class 2615 OID 16395)
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
-- TOC entry 3825 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: pg_database_owner
--

COMMENT ON SCHEMA public IS 'standard public schema';


--
-- TOC entry 11 (class 2615 OID 16393)
-- Name: students; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA students;


ALTER SCHEMA students OWNER TO test;

--
-- TOC entry 8 (class 2615 OID 16390)
-- Name: users; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA users;


ALTER SCHEMA users OWNER TO test;

--
-- TOC entry 289 (class 1255 OID 16869)
-- Name: update_updated_at_column(); Type: FUNCTION; Schema: public; Owner: test
--

CREATE FUNCTION public.update_updated_at_column() RETURNS trigger
    LANGUAGE plpgsql
    AS $$ BEGIN NEW.updated_at = CURRENT_TIMESTAMP; RETURN NEW; END; $$;


ALTER FUNCTION public.update_updated_at_column() OWNER TO test;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 235 (class 1259 OID 16576)
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
-- TOC entry 234 (class 1259 OID 16555)
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
    CONSTRAINT exams_status_check CHECK (((status)::text = ANY ((ARRAY['DRAFT'::character varying, 'PUBLISHED'::character varying, 'COMPLETED'::character varying, 'ARCHIVED'::character varying])::text[]))),
    CONSTRAINT exams_total_score_check CHECK ((total_score > (0)::numeric))
);


ALTER TABLE assessment.exams OWNER TO test;

--
-- TOC entry 233 (class 1259 OID 16538)
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
-- TOC entry 232 (class 1259 OID 16518)
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
    CONSTRAINT questions_difficulty_check CHECK (((difficulty)::text = ANY ((ARRAY['EASY'::character varying, 'MEDIUM'::character varying, 'HARD'::character varying, 'VERY_HARD'::character varying])::text[]))),
    CONSTRAINT questions_status_check CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'INACTIVE'::character varying, 'ARCHIVED'::character varying])::text[]))),
    CONSTRAINT questions_type_check CHECK (((type)::text = ANY ((ARRAY['MULTIPLE_CHOICE'::character varying, 'ESSAY'::character varying, 'SHORT_ANSWER'::character varying, 'TRUE_FALSE'::character varying])::text[])))
);


ALTER TABLE assessment.questions OWNER TO test;

--
-- TOC entry 227 (class 1259 OID 16435)
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
-- TOC entry 226 (class 1259 OID 16434)
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
-- TOC entry 3826 (class 0 OID 0)
-- Dependencies: 226
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: auth; Owner: test
--

ALTER SEQUENCE auth.roles_id_seq OWNED BY auth.roles.id;


--
-- TOC entry 229 (class 1259 OID 16466)
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
-- TOC entry 228 (class 1259 OID 16448)
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
-- TOC entry 238 (class 1259 OID 16648)
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
-- TOC entry 237 (class 1259 OID 16624)
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
    CONSTRAINT lesson_plans_grade_check CHECK ((grade = ANY (ARRAY[10, 11, 12]))),
    CONSTRAINT lesson_plans_status_check CHECK (((status)::text = ANY ((ARRAY['DRAFT'::character varying, 'COMPLETED'::character varying, 'PUBLISHED'::character varying, 'ARCHIVED'::character varying])::text[])))
);


ALTER TABLE content.lesson_plans OWNER TO test;

--
-- TOC entry 236 (class 1259 OID 16605)
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
    CONSTRAINT lesson_templates_status_check CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'INACTIVE'::character varying, 'ARCHIVED'::character varying])::text[])))
);


ALTER TABLE content.lesson_templates OWNER TO test;

--
-- TOC entry 244 (class 1259 OID 16785)
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
-- TOC entry 243 (class 1259 OID 16767)
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
    CONSTRAINT file_storage_file_type_check CHECK (((file_type)::text = ANY ((ARRAY['IMAGE'::character varying, 'DOCUMENT'::character varying, 'PDF'::character varying, 'EXCEL'::character varying, 'OTHER'::character varying])::text[]))),
    CONSTRAINT file_storage_status_check CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'ARCHIVED'::character varying, 'DELETED'::character varying])::text[])))
);


ALTER TABLE files.file_storage OWNER TO test;

--
-- TOC entry 248 (class 1259 OID 16853)
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
-- TOC entry 247 (class 1259 OID 16838)
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
    CONSTRAINT system_logs_level_check CHECK (((level)::text = ANY ((ARRAY['DEBUG'::character varying, 'INFO'::character varying, 'WARNING'::character varying, 'ERROR'::character varying, 'CRITICAL'::character varying])::text[])))
);


ALTER TABLE logging.system_logs OWNER TO test;

--
-- TOC entry 246 (class 1259 OID 16822)
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
    CONSTRAINT email_queue_status_check CHECK (((status)::text = ANY ((ARRAY['PENDING'::character varying, 'SENT'::character varying, 'FAILED'::character varying])::text[])))
);


ALTER TABLE notifications.email_queue OWNER TO test;

--
-- TOC entry 245 (class 1259 OID 16805)
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
    CONSTRAINT notifications_type_check CHECK (((type)::text = ANY ((ARRAY['INFO'::character varying, 'SUCCESS'::character varying, 'WARNING'::character varying, 'ERROR'::character varying])::text[])))
);


ALTER TABLE notifications.notifications OWNER TO test;

--
-- TOC entry 242 (class 1259 OID 16738)
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
    ocr_request_id uuid,
    ocr_processing_time integer,
    ocr_provider character varying(50),
    ocr_started_at timestamp without time zone,
    ocr_completed_at timestamp without time zone,
    ocr_error text,
    retry_count integer DEFAULT 0,
    max_retries integer DEFAULT 3,
    fallback_status character varying(50) DEFAULT NULL::character varying,
    fallback_reason text,
    manual_grading_required boolean DEFAULT false,
    CONSTRAINT answer_sheets_ocr_status_check CHECK (((ocr_status)::text = ANY ((ARRAY['PENDING'::character varying, 'PROCESSING'::character varying, 'COMPLETED'::character varying, 'FAILED'::character varying])::text[])))
);


ALTER TABLE students.answer_sheets OWNER TO test;

--
-- TOC entry 239 (class 1259 OID 16675)
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
-- TOC entry 241 (class 1259 OID 16714)
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
    ocr_result_id uuid,
    ocr_confidence numeric(3,2),
    fallback_note text,
    CONSTRAINT student_results_actual_duration_check CHECK ((actual_duration > 0)),
    CONSTRAINT student_results_grading_method_check CHECK (((grading_method)::text = ANY ((ARRAY['OCR'::character varying, 'MANUAL'::character varying, 'AUTO'::character varying])::text[]))),
    CONSTRAINT student_results_score_check CHECK ((score >= (0)::numeric))
);


ALTER TABLE students.student_results OWNER TO test;

--
-- TOC entry 240 (class 1259 OID 16692)
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
    CONSTRAINT students_gender_check CHECK (((gender)::text = ANY ((ARRAY['MALE'::character varying, 'FEMALE'::character varying, 'OTHER'::character varying])::text[])))
);


ALTER TABLE students.students OWNER TO test;

--
-- TOC entry 231 (class 1259 OID 16501)
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
-- TOC entry 252 (class 1259 OID 32774)
-- Name: ocr_rate_limits; Type: TABLE; Schema: users; Owner: test
--

CREATE TABLE users.ocr_rate_limits (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    teacher_id uuid,
    request_date date DEFAULT CURRENT_DATE,
    request_count integer DEFAULT 0,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE users.ocr_rate_limits OWNER TO test;

--
-- TOC entry 249 (class 1259 OID 16882)
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
    CONSTRAINT otp_codes_purpose_check CHECK (((purpose)::text = ANY ((ARRAY['PASSWORD_RESET'::character varying, 'EMAIL_VERIFICATION'::character varying, 'PHONE_VERIFICATION'::character varying])::text[])))
);


ALTER TABLE users.otp_codes OWNER TO test;

--
-- TOC entry 250 (class 1259 OID 16898)
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
-- TOC entry 230 (class 1259 OID 16486)
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
-- TOC entry 251 (class 1259 OID 16917)
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
-- TOC entry 3356 (class 2604 OID 16438)
-- Name: roles id; Type: DEFAULT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.roles ALTER COLUMN id SET DEFAULT nextval('auth.roles_id_seq'::regclass);


--
-- TOC entry 3801 (class 0 OID 16576)
-- Dependencies: 235
-- Data for Name: exam_questions; Type: TABLE DATA; Schema: assessment; Owner: test
--



--
-- TOC entry 3800 (class 0 OID 16555)
-- Dependencies: 234
-- Data for Name: exams; Type: TABLE DATA; Schema: assessment; Owner: test
--



--
-- TOC entry 3799 (class 0 OID 16538)
-- Dependencies: 233
-- Data for Name: question_choices; Type: TABLE DATA; Schema: assessment; Owner: test
--

INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('22f2599a-612b-4728-a180-a729e5405c48', 'f750e095-3782-4e0f-818c-2e4d97eec7b9', 'A', 'Háº¡t nhÃ¢n vÃ  electron', '2025-08-21 14:09:10.228645');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('acf59a5c-952b-436e-8582-18984db31859', 'f750e095-3782-4e0f-818c-2e4d97eec7b9', 'B', 'Chá»‰ cÃ³ háº¡t nhÃ¢n', '2025-08-21 14:09:10.228645');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('dd0248d2-2f77-4dd7-8299-cc4bf0ceb67d', 'f750e095-3782-4e0f-818c-2e4d97eec7b9', 'C', 'Chá»‰ cÃ³ electron', '2025-08-21 14:09:10.228645');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('f100bc5f-8760-4af5-a3c4-a80d82c1fab7', 'f750e095-3782-4e0f-818c-2e4d97eec7b9', 'D', 'Proton vÃ  neutron', '2025-08-21 14:09:10.228645');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('943af49a-145d-4988-ab0e-e7be19bd4fc5', '0419b04e-0960-4918-a78a-ed9836b87057', 'A', 'Theo khá»‘i lÆ°á»£ng nguyÃªn tá»­', '2025-08-21 14:09:10.228645');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('d20a338f-47a8-440f-aaf4-87ca89aab12b', '0419b04e-0960-4918-a78a-ed9836b87057', 'B', 'Theo sá»‘ hiá»‡u nguyÃªn tá»­ tÄƒng dáº§n', '2025-08-21 14:09:10.228645');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('9beab8aa-5824-42a7-a19a-cc54cca41999', '0419b04e-0960-4918-a78a-ed9836b87057', 'C', 'Theo tÃªn nguyÃªn tá»‘', '2025-08-21 14:09:10.228645');
INSERT INTO assessment.question_choices (id, question_id, choice_order, content, created_at) VALUES ('8221adf6-eed8-4baa-8036-22d9990772d4', '0419b04e-0960-4918-a78a-ed9836b87057', 'D', 'Theo mÃ u sáº¯c', '2025-08-21 14:09:10.228645');


--
-- TOC entry 3798 (class 0 OID 16518)
-- Dependencies: 232
-- Data for Name: questions; Type: TABLE DATA; Schema: assessment; Owner: test
--

INSERT INTO assessment.questions (id, content, type, difficulty, subject, topic, correct_answer, explanation, created_by, status, created_at, updated_at) VALUES ('f750e095-3782-4e0f-818c-2e4d97eec7b9', 'NguyÃªn tá»­ cÃ³ cáº¥u trÃºc nhÆ° tháº¿ nÃ o?', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 'Cáº¥u trÃºc nguyÃªn tá»­', 'A', 'NguyÃªn tá»­ gá»“m háº¡t nhÃ¢n vÃ  electron', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');
INSERT INTO assessment.questions (id, content, type, difficulty, subject, topic, correct_answer, explanation, created_by, status, created_at, updated_at) VALUES ('0419b04e-0960-4918-a78a-ed9836b87057', 'Trong báº£ng tuáº§n hoÃ n, cÃ¡c nguyÃªn tá»‘ Ä‘Æ°á»£c sáº¯p xáº¿p theo:', 'MULTIPLE_CHOICE', 'MEDIUM', 'HOA_HOC', 'Báº£ng tuáº§n hoÃ n', 'B', 'Theo sá»‘ hiá»‡u nguyÃªn tá»­ tÄƒng dáº§n', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');


--
-- TOC entry 3793 (class 0 OID 16435)
-- Dependencies: 227
-- Data for Name: roles; Type: TABLE DATA; Schema: auth; Owner: test
--

INSERT INTO auth.roles (id, name, description, is_active, created_at, updated_at) VALUES (1, 'ADMIN', 'Quáº£n trá»‹ viÃªn há»‡ thá»‘ng', true, '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');
INSERT INTO auth.roles (id, name, description, is_active, created_at, updated_at) VALUES (2, 'MANAGER', 'Quáº£n lÃ½ ná»™i dung vÃ  ngÆ°á»i dÃ¹ng', true, '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');
INSERT INTO auth.roles (id, name, description, is_active, created_at, updated_at) VALUES (3, 'STAFF', 'NhÃ¢n viÃªn táº¡o ná»™i dung máº«u', true, '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');
INSERT INTO auth.roles (id, name, description, is_active, created_at, updated_at) VALUES (4, 'TEACHER', 'GiÃ¡o viÃªn sá»­ dá»¥ng há»‡ thá»‘ng', true, '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');


--
-- TOC entry 3795 (class 0 OID 16466)
-- Dependencies: 229
-- Data for Name: sessions; Type: TABLE DATA; Schema: auth; Owner: test
--



--
-- TOC entry 3794 (class 0 OID 16448)
-- Dependencies: 228
-- Data for Name: users; Type: TABLE DATA; Schema: auth; Owner: test
--

INSERT INTO auth.users (id, email, password_hash, role_id, is_active, last_login, created_at, updated_at, deleted_at, is_deleted) VALUES ('550e8400-e29b-41d4-a716-446655440001', 'admin@planbookai.com', '$2a$06$66xG2IlpZbKwmabTpQHhKO9ZbY8nSAq2rFm68fVTAhM1GPJJ02hcW', 1, true, NULL, '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645', NULL, false);
INSERT INTO auth.users (id, email, password_hash, role_id, is_active, last_login, created_at, updated_at, deleted_at, is_deleted) VALUES ('550e8400-e29b-41d4-a716-446655440002', 'teacher@test.com', '$2a$06$sV/hvqG/wP9cAkaYRH.Do.frJdyLHP25Z7ISxA6wi8zANET3dYowi', 4, true, NULL, '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645', NULL, false);


--
-- TOC entry 3804 (class 0 OID 16648)
-- Dependencies: 238
-- Data for Name: chu_de; Type: TABLE DATA; Schema: content; Owner: test
--

INSERT INTO content.chu_de (id, name, description, subject, grade, parent_id, created_by, created_at, updated_at) VALUES ('922c18ca-489a-4fdf-a01a-1c5c2365a11e', 'Cáº¥u trÃºc nguyÃªn tá»­', 'NghiÃªn cá»©u vá» cáº¥u trÃºc vÃ  tÃ­nh cháº¥t cá»§a nguyÃªn tá»­', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');
INSERT INTO content.chu_de (id, name, description, subject, grade, parent_id, created_by, created_at, updated_at) VALUES ('a4514172-37bc-4377-addb-0239c16b54d4', 'Báº£ng tuáº§n hoÃ n', 'NghiÃªn cá»©u vá» báº£ng tuáº§n hoÃ n cÃ¡c nguyÃªn tá»‘ hÃ³a há»c', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');
INSERT INTO content.chu_de (id, name, description, subject, grade, parent_id, created_by, created_at, updated_at) VALUES ('d2ed63ff-d913-43f2-a54b-b053242705d3', 'LiÃªn káº¿t hÃ³a há»c', 'NghiÃªn cá»©u vá» cÃ¡c loáº¡i liÃªn káº¿t hÃ³a há»c', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');


--
-- TOC entry 3803 (class 0 OID 16624)
-- Dependencies: 237
-- Data for Name: lesson_plans; Type: TABLE DATA; Schema: content; Owner: test
--



--
-- TOC entry 3802 (class 0 OID 16605)
-- Dependencies: 236
-- Data for Name: lesson_templates; Type: TABLE DATA; Schema: content; Owner: test
--



--
-- TOC entry 3810 (class 0 OID 16785)
-- Dependencies: 244
-- Data for Name: file_metadata; Type: TABLE DATA; Schema: files; Owner: test
--



--
-- TOC entry 3809 (class 0 OID 16767)
-- Dependencies: 243
-- Data for Name: file_storage; Type: TABLE DATA; Schema: files; Owner: test
--



--
-- TOC entry 3814 (class 0 OID 16853)
-- Dependencies: 248
-- Data for Name: performance_metrics; Type: TABLE DATA; Schema: logging; Owner: test
--



--
-- TOC entry 3813 (class 0 OID 16838)
-- Dependencies: 247
-- Data for Name: system_logs; Type: TABLE DATA; Schema: logging; Owner: test
--



--
-- TOC entry 3812 (class 0 OID 16822)
-- Dependencies: 246
-- Data for Name: email_queue; Type: TABLE DATA; Schema: notifications; Owner: test
--



--
-- TOC entry 3811 (class 0 OID 16805)
-- Dependencies: 245
-- Data for Name: notifications; Type: TABLE DATA; Schema: notifications; Owner: test
--



--
-- TOC entry 3808 (class 0 OID 16738)
-- Dependencies: 242
-- Data for Name: answer_sheets; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3805 (class 0 OID 16675)
-- Dependencies: 239
-- Data for Name: classes; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3807 (class 0 OID 16714)
-- Dependencies: 241
-- Data for Name: student_results; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3806 (class 0 OID 16692)
-- Dependencies: 240
-- Data for Name: students; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3797 (class 0 OID 16501)
-- Dependencies: 231
-- Data for Name: activity_logs; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3818 (class 0 OID 32774)
-- Dependencies: 252
-- Data for Name: ocr_rate_limits; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3815 (class 0 OID 16882)
-- Dependencies: 249
-- Data for Name: otp_codes; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3816 (class 0 OID 16898)
-- Dependencies: 250
-- Data for Name: password_history; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3796 (class 0 OID 16486)
-- Dependencies: 230
-- Data for Name: user_profiles; Type: TABLE DATA; Schema: users; Owner: test
--

INSERT INTO users.user_profiles (id, user_id, full_name, phone, address, bio, avatar_url, created_at, updated_at) VALUES ('7d168c63-246d-4f8a-a6c7-635dbff6d6ac', '550e8400-e29b-41d4-a716-446655440001', 'Admin System', '0123456789', 'HÃ  Ná»™i', 'Quáº£n trá»‹ viÃªn há»‡ thá»‘ng', NULL, '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');
INSERT INTO users.user_profiles (id, user_id, full_name, phone, address, bio, avatar_url, created_at, updated_at) VALUES ('9a978ccf-127e-4293-9014-b58409e2e25c', '550e8400-e29b-41d4-a716-446655440002', 'GiÃ¡o viÃªn Test', '0987654321', 'TP.HCM', 'GiÃ¡o viÃªn HÃ³a há»c THPT', NULL, '2025-08-21 14:09:10.228645', '2025-08-21 14:09:10.228645');


--
-- TOC entry 3817 (class 0 OID 16917)
-- Dependencies: 251
-- Data for Name: user_sessions; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3827 (class 0 OID 0)
-- Dependencies: 226
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: auth; Owner: test
--

SELECT pg_catalog.setval('auth.roles_id_seq', 1, false);


--
-- TOC entry 3518 (class 2606 OID 16587)
-- Name: exam_questions exam_questions_exam_id_question_id_key; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_question_id_key UNIQUE (exam_id, question_id);


--
-- TOC entry 3520 (class 2606 OID 16585)
-- Name: exam_questions exam_questions_exam_id_question_order_key; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_question_order_key UNIQUE (exam_id, question_order);


--
-- TOC entry 3522 (class 2606 OID 16583)
-- Name: exam_questions exam_questions_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_pkey PRIMARY KEY (id);


--
-- TOC entry 3514 (class 2606 OID 16570)
-- Name: exams exams_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_pkey PRIMARY KEY (id);


--
-- TOC entry 3510 (class 2606 OID 16547)
-- Name: question_choices question_choices_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_pkey PRIMARY KEY (id);


--
-- TOC entry 3512 (class 2606 OID 16549)
-- Name: question_choices question_choices_question_id_choice_order_key; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_question_id_choice_order_key UNIQUE (question_id, choice_order);


--
-- TOC entry 3507 (class 2606 OID 16532)
-- Name: questions questions_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.questions
    ADD CONSTRAINT questions_pkey PRIMARY KEY (id);


--
-- TOC entry 3480 (class 2606 OID 16447)
-- Name: roles roles_name_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.roles
    ADD CONSTRAINT roles_name_key UNIQUE (name);


--
-- TOC entry 3482 (class 2606 OID 16445)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- TOC entry 3492 (class 2606 OID 16474)
-- Name: sessions sessions_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3494 (class 2606 OID 16476)
-- Name: sessions sessions_token_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_token_key UNIQUE (token);


--
-- TOC entry 3486 (class 2606 OID 16460)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 3488 (class 2606 OID 16458)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 3531 (class 2606 OID 16659)
-- Name: chu_de chu_de_pkey; Type: CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_pkey PRIMARY KEY (id);


--
-- TOC entry 3529 (class 2606 OID 16637)
-- Name: lesson_plans lesson_plans_pkey; Type: CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_pkey PRIMARY KEY (id);


--
-- TOC entry 3525 (class 2606 OID 16618)
-- Name: lesson_templates lesson_templates_pkey; Type: CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_templates
    ADD CONSTRAINT lesson_templates_pkey PRIMARY KEY (id);


--
-- TOC entry 3564 (class 2606 OID 16795)
-- Name: file_metadata file_metadata_file_id_key_key; Type: CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_file_id_key_key UNIQUE (file_id, key);


--
-- TOC entry 3566 (class 2606 OID 16793)
-- Name: file_metadata file_metadata_pkey; Type: CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_pkey PRIMARY KEY (id);


--
-- TOC entry 3559 (class 2606 OID 16779)
-- Name: file_storage file_storage_pkey; Type: CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_storage
    ADD CONSTRAINT file_storage_pkey PRIMARY KEY (id);


--
-- TOC entry 3583 (class 2606 OID 16859)
-- Name: performance_metrics performance_metrics_pkey; Type: CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.performance_metrics
    ADD CONSTRAINT performance_metrics_pkey PRIMARY KEY (id);


--
-- TOC entry 3579 (class 2606 OID 16847)
-- Name: system_logs system_logs_pkey; Type: CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.system_logs
    ADD CONSTRAINT system_logs_pkey PRIMARY KEY (id);


--
-- TOC entry 3574 (class 2606 OID 16833)
-- Name: email_queue email_queue_pkey; Type: CONSTRAINT; Schema: notifications; Owner: test
--

ALTER TABLE ONLY notifications.email_queue
    ADD CONSTRAINT email_queue_pkey PRIMARY KEY (id);


--
-- TOC entry 3572 (class 2606 OID 16816)
-- Name: notifications notifications_pkey; Type: CONSTRAINT; Schema: notifications; Owner: test
--

ALTER TABLE ONLY notifications.notifications
    ADD CONSTRAINT notifications_pkey PRIMARY KEY (id);


--
-- TOC entry 3552 (class 2606 OID 16748)
-- Name: answer_sheets answer_sheets_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_pkey PRIMARY KEY (id);


--
-- TOC entry 3535 (class 2606 OID 16686)
-- Name: classes classes_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.classes
    ADD CONSTRAINT classes_pkey PRIMARY KEY (id);


--
-- TOC entry 3548 (class 2606 OID 16725)
-- Name: student_results student_results_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_pkey PRIMARY KEY (id);


--
-- TOC entry 3550 (class 2606 OID 16727)
-- Name: student_results student_results_student_id_exam_id_key; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_student_id_exam_id_key UNIQUE (student_id, exam_id);


--
-- TOC entry 3541 (class 2606 OID 16703)
-- Name: students students_owner_teacher_id_student_code_key; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_owner_teacher_id_student_code_key UNIQUE (owner_teacher_id, student_code);


--
-- TOC entry 3543 (class 2606 OID 16701)
-- Name: students students_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_pkey PRIMARY KEY (id);


--
-- TOC entry 3499 (class 2606 OID 16509)
-- Name: activity_logs activity_logs_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.activity_logs
    ADD CONSTRAINT activity_logs_pkey PRIMARY KEY (id);


--
-- TOC entry 3601 (class 2606 OID 32783)
-- Name: ocr_rate_limits ocr_rate_limits_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.ocr_rate_limits
    ADD CONSTRAINT ocr_rate_limits_pkey PRIMARY KEY (id);


--
-- TOC entry 3603 (class 2606 OID 32785)
-- Name: ocr_rate_limits ocr_rate_limits_teacher_id_request_date_key; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.ocr_rate_limits
    ADD CONSTRAINT ocr_rate_limits_teacher_id_request_date_key UNIQUE (teacher_id, request_date);


--
-- TOC entry 3588 (class 2606 OID 16892)
-- Name: otp_codes otp_codes_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.otp_codes
    ADD CONSTRAINT otp_codes_pkey PRIMARY KEY (id);


--
-- TOC entry 3591 (class 2606 OID 16906)
-- Name: password_history password_history_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_pkey PRIMARY KEY (id);


--
-- TOC entry 3497 (class 2606 OID 16495)
-- Name: user_profiles user_profiles_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_profiles
    ADD CONSTRAINT user_profiles_pkey PRIMARY KEY (id);


--
-- TOC entry 3596 (class 2606 OID 16927)
-- Name: user_sessions user_sessions_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3598 (class 2606 OID 16929)
-- Name: user_sessions user_sessions_session_token_key; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_session_token_key UNIQUE (session_token);


--
-- TOC entry 3515 (class 1259 OID 16603)
-- Name: idx_assessment_exams_subject; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_exams_subject ON assessment.exams USING btree (subject);


--
-- TOC entry 3516 (class 1259 OID 16602)
-- Name: idx_assessment_exams_teacher; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_exams_teacher ON assessment.exams USING btree (teacher_id);


--
-- TOC entry 3508 (class 1259 OID 16604)
-- Name: idx_assessment_question_choices_question; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_question_choices_question ON assessment.question_choices USING btree (question_id);


--
-- TOC entry 3502 (class 1259 OID 16601)
-- Name: idx_assessment_questions_created_by; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_created_by ON assessment.questions USING btree (created_by);


--
-- TOC entry 3503 (class 1259 OID 16600)
-- Name: idx_assessment_questions_difficulty; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_difficulty ON assessment.questions USING btree (difficulty);


--
-- TOC entry 3504 (class 1259 OID 16598)
-- Name: idx_assessment_questions_subject; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_subject ON assessment.questions USING btree (subject);


--
-- TOC entry 3505 (class 1259 OID 16599)
-- Name: idx_assessment_questions_topic; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_topic ON assessment.questions USING btree (topic);


--
-- TOC entry 3489 (class 1259 OID 16485)
-- Name: idx_auth_sessions_token; Type: INDEX; Schema: auth; Owner: test
--

CREATE INDEX idx_auth_sessions_token ON auth.sessions USING btree (token);


--
-- TOC entry 3490 (class 1259 OID 16484)
-- Name: idx_auth_sessions_user; Type: INDEX; Schema: auth; Owner: test
--

CREATE INDEX idx_auth_sessions_user ON auth.sessions USING btree (user_id);


--
-- TOC entry 3483 (class 1259 OID 16482)
-- Name: idx_auth_users_email; Type: INDEX; Schema: auth; Owner: test
--

CREATE INDEX idx_auth_users_email ON auth.users USING btree (email);


--
-- TOC entry 3484 (class 1259 OID 16483)
-- Name: idx_auth_users_role; Type: INDEX; Schema: auth; Owner: test
--

CREATE INDEX idx_auth_users_role ON auth.users USING btree (role_id);


--
-- TOC entry 3532 (class 1259 OID 16674)
-- Name: idx_content_chu_de_parent; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_chu_de_parent ON content.chu_de USING btree (parent_id);


--
-- TOC entry 3533 (class 1259 OID 16673)
-- Name: idx_content_chu_de_subject; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_chu_de_subject ON content.chu_de USING btree (subject);


--
-- TOC entry 3526 (class 1259 OID 16672)
-- Name: idx_content_lesson_plans_subject; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_lesson_plans_subject ON content.lesson_plans USING btree (subject);


--
-- TOC entry 3527 (class 1259 OID 16671)
-- Name: idx_content_lesson_plans_teacher; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_lesson_plans_teacher ON content.lesson_plans USING btree (teacher_id);


--
-- TOC entry 3523 (class 1259 OID 16670)
-- Name: idx_content_lesson_templates_subject; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_lesson_templates_subject ON content.lesson_templates USING btree (subject);


--
-- TOC entry 3567 (class 1259 OID 16804)
-- Name: idx_files_metadata_file; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_metadata_file ON files.file_metadata USING btree (file_id);


--
-- TOC entry 3560 (class 1259 OID 16802)
-- Name: idx_files_storage_file_type; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_storage_file_type ON files.file_storage USING btree (file_type);


--
-- TOC entry 3561 (class 1259 OID 16803)
-- Name: idx_files_storage_status; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_storage_status ON files.file_storage USING btree (status);


--
-- TOC entry 3562 (class 1259 OID 16801)
-- Name: idx_files_storage_uploaded_by; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_storage_uploaded_by ON files.file_storage USING btree (uploaded_by);


--
-- TOC entry 3580 (class 1259 OID 16868)
-- Name: idx_logging_performance_created_at; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_performance_created_at ON logging.performance_metrics USING btree (created_at);


--
-- TOC entry 3581 (class 1259 OID 16867)
-- Name: idx_logging_performance_service; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_performance_service ON logging.performance_metrics USING btree (service_name);


--
-- TOC entry 3576 (class 1259 OID 16866)
-- Name: idx_logging_system_logs_created_at; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_system_logs_created_at ON logging.system_logs USING btree (created_at);


--
-- TOC entry 3577 (class 1259 OID 16865)
-- Name: idx_logging_system_logs_level; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_system_logs_level ON logging.system_logs USING btree (level);


--
-- TOC entry 3575 (class 1259 OID 16837)
-- Name: idx_email_queue_status; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_email_queue_status ON notifications.email_queue USING btree (status);


--
-- TOC entry 3568 (class 1259 OID 16836)
-- Name: idx_notifications_created_at; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_notifications_created_at ON notifications.notifications USING btree (created_at);


--
-- TOC entry 3569 (class 1259 OID 16835)
-- Name: idx_notifications_is_read; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_notifications_is_read ON notifications.notifications USING btree (is_read);


--
-- TOC entry 3570 (class 1259 OID 16834)
-- Name: idx_notifications_user; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_notifications_user ON notifications.notifications USING btree (user_id);


--
-- TOC entry 3553 (class 1259 OID 32773)
-- Name: idx_answer_sheets_fallback_status; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_answer_sheets_fallback_status ON students.answer_sheets USING btree (fallback_status);


--
-- TOC entry 3554 (class 1259 OID 24576)
-- Name: idx_answer_sheets_ocr_request; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_answer_sheets_ocr_request ON students.answer_sheets USING btree (ocr_request_id);


--
-- TOC entry 3555 (class 1259 OID 32772)
-- Name: idx_answer_sheets_retry_count; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_answer_sheets_retry_count ON students.answer_sheets USING btree (retry_count);


--
-- TOC entry 3544 (class 1259 OID 24577)
-- Name: idx_student_results_ocr_result; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_student_results_ocr_result ON students.student_results USING btree (ocr_result_id);


--
-- TOC entry 3556 (class 1259 OID 16766)
-- Name: idx_students_answer_sheets_exam; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_answer_sheets_exam ON students.answer_sheets USING btree (exam_id);


--
-- TOC entry 3557 (class 1259 OID 16765)
-- Name: idx_students_answer_sheets_student; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_answer_sheets_student ON students.answer_sheets USING btree (student_id);


--
-- TOC entry 3536 (class 1259 OID 16759)
-- Name: idx_students_classes_teacher; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_classes_teacher ON students.classes USING btree (homeroom_teacher_id);


--
-- TOC entry 3545 (class 1259 OID 16764)
-- Name: idx_students_student_results_exam; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_student_results_exam ON students.student_results USING btree (exam_id);


--
-- TOC entry 3546 (class 1259 OID 16763)
-- Name: idx_students_student_results_student; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_student_results_student ON students.student_results USING btree (student_id);


--
-- TOC entry 3537 (class 1259 OID 16760)
-- Name: idx_students_students_class; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_students_class ON students.students USING btree (class_id);


--
-- TOC entry 3538 (class 1259 OID 16762)
-- Name: idx_students_students_code; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_students_code ON students.students USING btree (student_code);


--
-- TOC entry 3539 (class 1259 OID 16761)
-- Name: idx_students_students_teacher; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_students_teacher ON students.students USING btree (owner_teacher_id);


--
-- TOC entry 3599 (class 1259 OID 32791)
-- Name: idx_ocr_rate_limits_teacher_date; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_ocr_rate_limits_teacher_date ON users.ocr_rate_limits USING btree (teacher_id, request_date);


--
-- TOC entry 3500 (class 1259 OID 16517)
-- Name: idx_users_activity_logs_created; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_activity_logs_created ON users.activity_logs USING btree (created_at);


--
-- TOC entry 3501 (class 1259 OID 16516)
-- Name: idx_users_activity_logs_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_activity_logs_user ON users.activity_logs USING btree (user_id);


--
-- TOC entry 3584 (class 1259 OID 16937)
-- Name: idx_users_otp_codes_expires; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_otp_codes_expires ON users.otp_codes USING btree (expires_at);


--
-- TOC entry 3585 (class 1259 OID 16936)
-- Name: idx_users_otp_codes_purpose; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_otp_codes_purpose ON users.otp_codes USING btree (purpose);


--
-- TOC entry 3586 (class 1259 OID 16935)
-- Name: idx_users_otp_codes_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_otp_codes_user ON users.otp_codes USING btree (user_id);


--
-- TOC entry 3589 (class 1259 OID 16938)
-- Name: idx_users_password_history_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_password_history_user ON users.password_history USING btree (user_id);


--
-- TOC entry 3495 (class 1259 OID 16515)
-- Name: idx_users_profiles_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_profiles_user ON users.user_profiles USING btree (user_id);


--
-- TOC entry 3592 (class 1259 OID 16941)
-- Name: idx_users_user_sessions_active; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_user_sessions_active ON users.user_sessions USING btree (is_active);


--
-- TOC entry 3593 (class 1259 OID 16940)
-- Name: idx_users_user_sessions_token; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_user_sessions_token ON users.user_sessions USING btree (session_token);


--
-- TOC entry 3594 (class 1259 OID 16939)
-- Name: idx_users_user_sessions_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_user_sessions_user ON users.user_sessions USING btree (user_id);


--
-- TOC entry 3640 (class 2620 OID 16874)
-- Name: exams trigger_updated_at_assessment_exams; Type: TRIGGER; Schema: assessment; Owner: test
--

CREATE TRIGGER trigger_updated_at_assessment_exams BEFORE UPDATE ON assessment.exams FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3639 (class 2620 OID 16873)
-- Name: questions trigger_updated_at_assessment_questions; Type: TRIGGER; Schema: assessment; Owner: test
--

CREATE TRIGGER trigger_updated_at_assessment_questions BEFORE UPDATE ON assessment.questions FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3635 (class 2620 OID 16870)
-- Name: roles trigger_updated_at_auth_roles; Type: TRIGGER; Schema: auth; Owner: test
--

CREATE TRIGGER trigger_updated_at_auth_roles BEFORE UPDATE ON auth.roles FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3636 (class 2620 OID 16871)
-- Name: users trigger_updated_at_auth_users; Type: TRIGGER; Schema: auth; Owner: test
--

CREATE TRIGGER trigger_updated_at_auth_users BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3637 (class 2620 OID 16942)
-- Name: users trigger_updated_at_auth_users_soft_delete; Type: TRIGGER; Schema: auth; Owner: test
--

CREATE TRIGGER trigger_updated_at_auth_users_soft_delete BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3643 (class 2620 OID 16877)
-- Name: chu_de trigger_updated_at_content_chu_de; Type: TRIGGER; Schema: content; Owner: test
--

CREATE TRIGGER trigger_updated_at_content_chu_de BEFORE UPDATE ON content.chu_de FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3642 (class 2620 OID 16876)
-- Name: lesson_plans trigger_updated_at_content_lesson_plans; Type: TRIGGER; Schema: content; Owner: test
--

CREATE TRIGGER trigger_updated_at_content_lesson_plans BEFORE UPDATE ON content.lesson_plans FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3641 (class 2620 OID 16875)
-- Name: lesson_templates trigger_updated_at_content_lesson_templates; Type: TRIGGER; Schema: content; Owner: test
--

CREATE TRIGGER trigger_updated_at_content_lesson_templates BEFORE UPDATE ON content.lesson_templates FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3646 (class 2620 OID 16880)
-- Name: file_storage trigger_updated_at_files_file_storage; Type: TRIGGER; Schema: files; Owner: test
--

CREATE TRIGGER trigger_updated_at_files_file_storage BEFORE UPDATE ON files.file_storage FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3644 (class 2620 OID 16878)
-- Name: classes trigger_updated_at_students_classes; Type: TRIGGER; Schema: students; Owner: test
--

CREATE TRIGGER trigger_updated_at_students_classes BEFORE UPDATE ON students.classes FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3645 (class 2620 OID 16879)
-- Name: students trigger_updated_at_students_students; Type: TRIGGER; Schema: students; Owner: test
--

CREATE TRIGGER trigger_updated_at_students_students BEFORE UPDATE ON students.students FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3638 (class 2620 OID 16872)
-- Name: user_profiles trigger_updated_at_users_user_profiles; Type: TRIGGER; Schema: users; Owner: test
--

CREATE TRIGGER trigger_updated_at_users_user_profiles BEFORE UPDATE ON users.user_profiles FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3611 (class 2606 OID 16588)
-- Name: exam_questions exam_questions_exam_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id) ON DELETE CASCADE;


--
-- TOC entry 3612 (class 2606 OID 16593)
-- Name: exam_questions exam_questions_question_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_question_id_fkey FOREIGN KEY (question_id) REFERENCES assessment.questions(id);


--
-- TOC entry 3610 (class 2606 OID 16571)
-- Name: exams exams_teacher_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3609 (class 2606 OID 16550)
-- Name: question_choices question_choices_question_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_question_id_fkey FOREIGN KEY (question_id) REFERENCES assessment.questions(id) ON DELETE CASCADE;


--
-- TOC entry 3608 (class 2606 OID 16533)
-- Name: questions questions_created_by_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.questions
    ADD CONSTRAINT questions_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3605 (class 2606 OID 16477)
-- Name: sessions sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3604 (class 2606 OID 16461)
-- Name: users users_role_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES auth.roles(id);


--
-- TOC entry 3616 (class 2606 OID 16665)
-- Name: chu_de chu_de_created_by_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3617 (class 2606 OID 16660)
-- Name: chu_de chu_de_parent_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_parent_id_fkey FOREIGN KEY (parent_id) REFERENCES content.chu_de(id);


--
-- TOC entry 3614 (class 2606 OID 16638)
-- Name: lesson_plans lesson_plans_teacher_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3615 (class 2606 OID 16643)
-- Name: lesson_plans lesson_plans_template_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_template_id_fkey FOREIGN KEY (template_id) REFERENCES content.lesson_templates(id);


--
-- TOC entry 3613 (class 2606 OID 16619)
-- Name: lesson_templates lesson_templates_created_by_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_templates
    ADD CONSTRAINT lesson_templates_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3626 (class 2606 OID 16796)
-- Name: file_metadata file_metadata_file_id_fkey; Type: FK CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_file_id_fkey FOREIGN KEY (file_id) REFERENCES files.file_storage(id) ON DELETE CASCADE;


--
-- TOC entry 3625 (class 2606 OID 16780)
-- Name: file_storage file_storage_uploaded_by_fkey; Type: FK CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_storage
    ADD CONSTRAINT file_storage_uploaded_by_fkey FOREIGN KEY (uploaded_by) REFERENCES auth.users(id);


--
-- TOC entry 3629 (class 2606 OID 16860)
-- Name: performance_metrics performance_metrics_user_id_fkey; Type: FK CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.performance_metrics
    ADD CONSTRAINT performance_metrics_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3628 (class 2606 OID 16848)
-- Name: system_logs system_logs_user_id_fkey; Type: FK CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.system_logs
    ADD CONSTRAINT system_logs_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3627 (class 2606 OID 16817)
-- Name: notifications notifications_user_id_fkey; Type: FK CONSTRAINT; Schema: notifications; Owner: test
--

ALTER TABLE ONLY notifications.notifications
    ADD CONSTRAINT notifications_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3623 (class 2606 OID 16754)
-- Name: answer_sheets answer_sheets_exam_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id);


--
-- TOC entry 3624 (class 2606 OID 16749)
-- Name: answer_sheets answer_sheets_student_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_student_id_fkey FOREIGN KEY (student_id) REFERENCES students.students(id);


--
-- TOC entry 3618 (class 2606 OID 16687)
-- Name: classes classes_homeroom_teacher_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.classes
    ADD CONSTRAINT classes_homeroom_teacher_id_fkey FOREIGN KEY (homeroom_teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3621 (class 2606 OID 16733)
-- Name: student_results student_results_exam_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id);


--
-- TOC entry 3622 (class 2606 OID 16728)
-- Name: student_results student_results_student_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_student_id_fkey FOREIGN KEY (student_id) REFERENCES students.students(id);


--
-- TOC entry 3619 (class 2606 OID 16704)
-- Name: students students_class_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_class_id_fkey FOREIGN KEY (class_id) REFERENCES students.classes(id);


--
-- TOC entry 3620 (class 2606 OID 16709)
-- Name: students students_owner_teacher_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_owner_teacher_id_fkey FOREIGN KEY (owner_teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3607 (class 2606 OID 16510)
-- Name: activity_logs activity_logs_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.activity_logs
    ADD CONSTRAINT activity_logs_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3634 (class 2606 OID 32786)
-- Name: ocr_rate_limits ocr_rate_limits_teacher_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.ocr_rate_limits
    ADD CONSTRAINT ocr_rate_limits_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3630 (class 2606 OID 16893)
-- Name: otp_codes otp_codes_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.otp_codes
    ADD CONSTRAINT otp_codes_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3631 (class 2606 OID 16912)
-- Name: password_history password_history_changed_by_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_changed_by_fkey FOREIGN KEY (changed_by) REFERENCES auth.users(id);


--
-- TOC entry 3632 (class 2606 OID 16907)
-- Name: password_history password_history_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3606 (class 2606 OID 16496)
-- Name: user_profiles user_profiles_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_profiles
    ADD CONSTRAINT user_profiles_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3633 (class 2606 OID 16930)
-- Name: user_sessions user_sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


-- Completed on 2025-08-31 23:12:58

--
-- PostgreSQL database dump complete
--
