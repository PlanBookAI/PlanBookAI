--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5 (Debian 17.5-1.pgdg120+1)
-- Dumped by pg_dump version 17.5

-- Started on 2025-08-19 03:14:29

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
-- Name: assessment; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA assessment;


ALTER SCHEMA assessment OWNER TO test;

--
-- TOC entry 7 (class 2615 OID 16385)
-- Name: auth; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA auth;


ALTER SCHEMA auth OWNER TO test;

--
-- TOC entry 10 (class 2615 OID 16388)
-- Name: content; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA content;


ALTER SCHEMA content OWNER TO test;

--
-- TOC entry 12 (class 2615 OID 16390)
-- Name: files; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA files;


ALTER SCHEMA files OWNER TO test;

--
-- TOC entry 14 (class 2615 OID 16392)
-- Name: logging; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA logging;


ALTER SCHEMA logging OWNER TO test;

--
-- TOC entry 13 (class 2615 OID 16391)
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
-- TOC entry 3850 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: pg_database_owner
--

COMMENT ON SCHEMA public IS 'standard public schema';


--
-- TOC entry 11 (class 2615 OID 16389)
-- Name: students; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA students;


ALTER SCHEMA students OWNER TO test;

--
-- TOC entry 8 (class 2615 OID 16386)
-- Name: users; Type: SCHEMA; Schema: -; Owner: test
--

CREATE SCHEMA users;


ALTER SCHEMA users OWNER TO test;

--
-- TOC entry 292 (class 1255 OID 16865)
-- Name: update_updated_at_column(); Type: FUNCTION; Schema: public; Owner: test
--

CREATE FUNCTION public.update_updated_at_column() RETURNS trigger
    LANGUAGE plpgsql
    AS $$ BEGIN NEW.updated_at = CURRENT_TIMESTAMP; RETURN NEW; END; $$;


ALTER FUNCTION public.update_updated_at_column() OWNER TO test;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 235 (class 1259 OID 16572)
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
-- TOC entry 234 (class 1259 OID 16551)
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
-- TOC entry 233 (class 1259 OID 16534)
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
-- TOC entry 232 (class 1259 OID 16514)
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
-- TOC entry 255 (class 1259 OID 16997)
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
-- TOC entry 254 (class 1259 OID 16980)
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
-- TOC entry 252 (class 1259 OID 16948)
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
-- TOC entry 227 (class 1259 OID 16431)
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
-- TOC entry 226 (class 1259 OID 16430)
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
-- TOC entry 3851 (class 0 OID 0)
-- Dependencies: 226
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: auth; Owner: test
--

ALTER SEQUENCE auth.roles_id_seq OWNED BY auth.roles.id;


--
-- TOC entry 229 (class 1259 OID 16462)
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
-- TOC entry 253 (class 1259 OID 16962)
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
-- TOC entry 228 (class 1259 OID 16444)
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
-- TOC entry 238 (class 1259 OID 16644)
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
-- TOC entry 237 (class 1259 OID 16620)
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
-- TOC entry 236 (class 1259 OID 16601)
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
-- TOC entry 244 (class 1259 OID 16781)
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
-- TOC entry 243 (class 1259 OID 16763)
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
-- TOC entry 248 (class 1259 OID 16849)
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
-- TOC entry 247 (class 1259 OID 16834)
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
-- TOC entry 246 (class 1259 OID 16818)
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
-- TOC entry 245 (class 1259 OID 16801)
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
-- TOC entry 242 (class 1259 OID 16734)
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
    CONSTRAINT answer_sheets_ocr_status_check CHECK (((ocr_status)::text = ANY ((ARRAY['PENDING'::character varying, 'PROCESSING'::character varying, 'COMPLETED'::character varying, 'FAILED'::character varying])::text[])))
);


ALTER TABLE students.answer_sheets OWNER TO test;

--
-- TOC entry 239 (class 1259 OID 16671)
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
-- TOC entry 241 (class 1259 OID 16710)
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
    CONSTRAINT student_results_grading_method_check CHECK (((grading_method)::text = ANY ((ARRAY['OCR'::character varying, 'MANUAL'::character varying, 'AUTO'::character varying])::text[]))),
    CONSTRAINT student_results_score_check CHECK ((score >= (0)::numeric))
);


ALTER TABLE students.student_results OWNER TO test;

--
-- TOC entry 240 (class 1259 OID 16688)
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
-- TOC entry 231 (class 1259 OID 16497)
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
-- TOC entry 249 (class 1259 OID 16878)
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
-- TOC entry 250 (class 1259 OID 16894)
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
-- TOC entry 230 (class 1259 OID 16482)
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
-- TOC entry 251 (class 1259 OID 16913)
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
-- TOC entry 3368 (class 2604 OID 16434)
-- Name: roles id; Type: DEFAULT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.roles ALTER COLUMN id SET DEFAULT nextval('auth.roles_id_seq'::regclass);


--
-- TOC entry 3824 (class 0 OID 16572)
-- Dependencies: 235
-- Data for Name: exam_questions; Type: TABLE DATA; Schema: assessment; Owner: test
--



--
-- TOC entry 3823 (class 0 OID 16551)
-- Dependencies: 234
-- Data for Name: exams; Type: TABLE DATA; Schema: assessment; Owner: test
--



--
-- TOC entry 3822 (class 0 OID 16534)
-- Dependencies: 233
-- Data for Name: question_choices; Type: TABLE DATA; Schema: assessment; Owner: test
--

INSERT INTO assessment.question_choices VALUES ('eb0d44bc-d63c-48c7-8978-a9d206e5643a', '1c450f14-a7fb-4608-933d-bee1a85705d7', 'A', 'Háº¡t nhÃ¢n vÃ  electron', '2025-08-18 19:07:09.558502');
INSERT INTO assessment.question_choices VALUES ('7aaa6f72-942f-4b8c-91d8-14e421140058', '1c450f14-a7fb-4608-933d-bee1a85705d7', 'B', 'Chá»‰ cÃ³ háº¡t nhÃ¢n', '2025-08-18 19:07:09.558502');
INSERT INTO assessment.question_choices VALUES ('585910fc-fb9b-40d5-b1e5-840ff6ea6ace', '1c450f14-a7fb-4608-933d-bee1a85705d7', 'C', 'Chá»‰ cÃ³ electron', '2025-08-18 19:07:09.558502');
INSERT INTO assessment.question_choices VALUES ('1c27d3f8-daac-405b-8123-14cc7a661104', '1c450f14-a7fb-4608-933d-bee1a85705d7', 'D', 'Proton vÃ  neutron', '2025-08-18 19:07:09.558502');
INSERT INTO assessment.question_choices VALUES ('fdc531f9-8f21-47db-8ad0-8befbc2c176c', '054e4b22-3fd3-4dc2-b2d9-eec82c2d3810', 'A', 'Theo khá»‘i lÆ°á»£ng nguyÃªn tá»­', '2025-08-18 19:07:09.558502');
INSERT INTO assessment.question_choices VALUES ('b4f65982-c70d-48e3-97a7-eeb3afa89b0c', '054e4b22-3fd3-4dc2-b2d9-eec82c2d3810', 'B', 'Theo sá»‘ hiá»‡u nguyÃªn tá»­ tÄƒng dáº§n', '2025-08-18 19:07:09.558502');
INSERT INTO assessment.question_choices VALUES ('08f6f68e-97df-457e-bf78-cb47311538f9', '054e4b22-3fd3-4dc2-b2d9-eec82c2d3810', 'C', 'Theo tÃªn nguyÃªn tá»‘', '2025-08-18 19:07:09.558502');
INSERT INTO assessment.question_choices VALUES ('34db39c3-589e-4edf-8e65-e7bc934628bd', '054e4b22-3fd3-4dc2-b2d9-eec82c2d3810', 'D', 'Theo mÃ u sáº¯c', '2025-08-18 19:07:09.558502');
INSERT INTO assessment.question_choices VALUES ('9de4d75d-d13f-49a4-bece-cdf10432b397', 'c4aedb60-f2ab-451d-8b46-e98039201a1b', 'A', 'Háº¡t nhÃ¢n vÃ  electron', '2025-08-18 19:38:41.837025');
INSERT INTO assessment.question_choices VALUES ('4f054581-5bc0-4ccd-b1b7-c922ec3f1768', 'c4aedb60-f2ab-451d-8b46-e98039201a1b', 'B', 'Chá»‰ cÃ³ háº¡t nhÃ¢n', '2025-08-18 19:38:41.837025');
INSERT INTO assessment.question_choices VALUES ('0d8fd1e6-c6da-4302-bd72-9f5ae0952877', 'c4aedb60-f2ab-451d-8b46-e98039201a1b', 'C', 'Chá»‰ cÃ³ electron', '2025-08-18 19:38:41.837025');
INSERT INTO assessment.question_choices VALUES ('1addd262-7392-408f-9112-8e11c7ec9cf2', 'c4aedb60-f2ab-451d-8b46-e98039201a1b', 'D', 'Proton vÃ  neutron', '2025-08-18 19:38:41.837025');
INSERT INTO assessment.question_choices VALUES ('d30365af-572e-4ff2-8d10-92fe143884d9', '08ab7db5-2417-42d7-9146-01a348fd0d5a', 'A', 'Theo khá»‘i lÆ°á»£ng nguyÃªn tá»­', '2025-08-18 19:38:41.837025');
INSERT INTO assessment.question_choices VALUES ('d23b6dbd-7ccb-457a-a6fe-fd1197ab6bd9', '08ab7db5-2417-42d7-9146-01a348fd0d5a', 'B', 'Theo sá»‘ hiá»‡u nguyÃªn tá»­ tÄƒng dáº§n', '2025-08-18 19:38:41.837025');
INSERT INTO assessment.question_choices VALUES ('3f6024d9-80bd-4e0e-ae63-3a164cc841f7', '08ab7db5-2417-42d7-9146-01a348fd0d5a', 'C', 'Theo tÃªn nguyÃªn tá»‘', '2025-08-18 19:38:41.837025');
INSERT INTO assessment.question_choices VALUES ('31c78eaf-e71a-474c-ba0f-6daf6a9164af', '08ab7db5-2417-42d7-9146-01a348fd0d5a', 'D', 'Theo mÃ u sáº¯c', '2025-08-18 19:38:41.837025');
INSERT INTO assessment.question_choices VALUES ('c9d65b30-f4fb-4bcd-8aab-185f0be38cb4', '05743b3a-da49-438f-a85a-a8e4c4baadff', 'A', 'Háº¡t nhÃ¢n vÃ  electron', '2025-08-18 19:43:34.099182');
INSERT INTO assessment.question_choices VALUES ('d8b00a99-00ac-4c04-8af2-b31bcc2bc2c4', '05743b3a-da49-438f-a85a-a8e4c4baadff', 'B', 'Chá»‰ cÃ³ háº¡t nhÃ¢n', '2025-08-18 19:43:34.099182');
INSERT INTO assessment.question_choices VALUES ('e64efc28-215f-4910-8a37-319416cd6362', '05743b3a-da49-438f-a85a-a8e4c4baadff', 'C', 'Chá»‰ cÃ³ electron', '2025-08-18 19:43:34.099182');
INSERT INTO assessment.question_choices VALUES ('c5c9149c-3f5e-49fd-b86e-c5b2f248742a', '05743b3a-da49-438f-a85a-a8e4c4baadff', 'D', 'Proton vÃ  neutron', '2025-08-18 19:43:34.099182');
INSERT INTO assessment.question_choices VALUES ('48d947f8-8c22-47a1-9dfb-7e13f87eb389', 'bbf5fe70-636a-44a6-aa3b-692cc73d207f', 'A', 'Theo khá»‘i lÆ°á»£ng nguyÃªn tá»­', '2025-08-18 19:43:34.099182');
INSERT INTO assessment.question_choices VALUES ('90f49d98-29f8-4681-af9e-e66ff37dfb1e', 'bbf5fe70-636a-44a6-aa3b-692cc73d207f', 'B', 'Theo sá»‘ hiá»‡u nguyÃªn tá»­ tÄƒng dáº§n', '2025-08-18 19:43:34.099182');
INSERT INTO assessment.question_choices VALUES ('9fd6b6f8-6b50-4121-8ca5-0c242b650755', 'bbf5fe70-636a-44a6-aa3b-692cc73d207f', 'C', 'Theo tÃªn nguyÃªn tá»‘', '2025-08-18 19:43:34.099182');
INSERT INTO assessment.question_choices VALUES ('9de4194c-35e8-47c9-9ed0-0698b1eb8eff', 'bbf5fe70-636a-44a6-aa3b-692cc73d207f', 'D', 'Theo mÃ u sáº¯c', '2025-08-18 19:43:34.099182');


--
-- TOC entry 3821 (class 0 OID 16514)
-- Dependencies: 232
-- Data for Name: questions; Type: TABLE DATA; Schema: assessment; Owner: test
--

INSERT INTO assessment.questions VALUES ('1c450f14-a7fb-4608-933d-bee1a85705d7', 'NguyÃªn tá»­ cÃ³ cáº¥u trÃºc nhÆ° tháº¿ nÃ o?', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 'Cáº¥u trÃºc nguyÃªn tá»­', 'A', 'NguyÃªn tá»­ gá»“m háº¡t nhÃ¢n vÃ  electron', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');
INSERT INTO assessment.questions VALUES ('054e4b22-3fd3-4dc2-b2d9-eec82c2d3810', 'Trong báº£ng tuáº§n hoÃ n, cÃ¡c nguyÃªn tá»‘ Ä‘Æ°á»£c sáº¯p xáº¿p theo:', 'MULTIPLE_CHOICE', 'MEDIUM', 'HOA_HOC', 'Báº£ng tuáº§n hoÃ n', 'B', 'Theo sá»‘ hiá»‡u nguyÃªn tá»­ tÄƒng dáº§n', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');
INSERT INTO assessment.questions VALUES ('c4aedb60-f2ab-451d-8b46-e98039201a1b', 'NguyÃªn tá»­ cÃ³ cáº¥u trÃºc nhÆ° tháº¿ nÃ o?', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 'Cáº¥u trÃºc nguyÃªn tá»­', 'A', 'NguyÃªn tá»­ gá»“m háº¡t nhÃ¢n vÃ  electron', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-08-18 19:38:41.837025', '2025-08-18 19:38:41.837025');
INSERT INTO assessment.questions VALUES ('08ab7db5-2417-42d7-9146-01a348fd0d5a', 'Trong báº£ng tuáº§n hoÃ n, cÃ¡c nguyÃªn tá»‘ Ä‘Æ°á»£c sáº¯p xáº¿p theo:', 'MULTIPLE_CHOICE', 'MEDIUM', 'HOA_HOC', 'Báº£ng tuáº§n hoÃ n', 'B', 'Theo sá»‘ hiá»‡u nguyÃªn tá»­ tÄƒng dáº§n', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-08-18 19:38:41.837025', '2025-08-18 19:38:41.837025');
INSERT INTO assessment.questions VALUES ('05743b3a-da49-438f-a85a-a8e4c4baadff', 'NguyÃªn tá»­ cÃ³ cáº¥u trÃºc nhÆ° tháº¿ nÃ o?', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 'Cáº¥u trÃºc nguyÃªn tá»­', 'A', 'NguyÃªn tá»­ gá»“m háº¡t nhÃ¢n vÃ  electron', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-08-18 19:43:34.099182', '2025-08-18 19:43:34.099182');
INSERT INTO assessment.questions VALUES ('bbf5fe70-636a-44a6-aa3b-692cc73d207f', 'Trong báº£ng tuáº§n hoÃ n, cÃ¡c nguyÃªn tá»‘ Ä‘Æ°á»£c sáº¯p xáº¿p theo:', 'MULTIPLE_CHOICE', 'MEDIUM', 'HOA_HOC', 'Báº£ng tuáº§n hoÃ n', 'B', 'Theo sá»‘ hiá»‡u nguyÃªn tá»­ tÄƒng dáº§n', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-08-18 19:43:34.099182', '2025-08-18 19:43:34.099182');


--
-- TOC entry 3844 (class 0 OID 16997)
-- Dependencies: 255
-- Data for Name: email_verifications; Type: TABLE DATA; Schema: auth; Owner: test
--



--
-- TOC entry 3843 (class 0 OID 16980)
-- Dependencies: 254
-- Data for Name: password_history; Type: TABLE DATA; Schema: auth; Owner: test
--



--
-- TOC entry 3841 (class 0 OID 16948)
-- Dependencies: 252
-- Data for Name: password_resets; Type: TABLE DATA; Schema: auth; Owner: test
--



--
-- TOC entry 3816 (class 0 OID 16431)
-- Dependencies: 227
-- Data for Name: roles; Type: TABLE DATA; Schema: auth; Owner: test
--

INSERT INTO auth.roles VALUES (1, 'ADMIN', 'Quáº£n trá»‹ viÃªn há»‡ thá»‘ng', true, '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');
INSERT INTO auth.roles VALUES (2, 'MANAGER', 'Quáº£n lÃ½ ná»™i dung vÃ  ngÆ°á»i dÃ¹ng', true, '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');
INSERT INTO auth.roles VALUES (3, 'STAFF', 'NhÃ¢n viÃªn táº¡o ná»™i dung máº«u', true, '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');
INSERT INTO auth.roles VALUES (4, 'TEACHER', 'GiÃ¡o viÃªn sá»­ dá»¥ng há»‡ thá»‘ng', true, '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');


--
-- TOC entry 3818 (class 0 OID 16462)
-- Dependencies: 229
-- Data for Name: sessions; Type: TABLE DATA; Schema: auth; Owner: test
--

INSERT INTO auth.sessions VALUES ('8d8b7fcc-6780-43e6-92d5-9fbe10639858', '04c40fc3-4c99-497d-945e-aa1f6c483549', 'ATYx8S864hcfqYU4x9wF8eRB1sdXKIVe4glKwMO3EqpZD4QoD5ooMYTefSwtY5SXipIBCaMD2f9CYt1gr8vUWA==', '2025-08-25 19:37:31.973832', '2025-08-18 19:37:31.973762');
INSERT INTO auth.sessions VALUES ('037ea85c-0fed-459f-9c0b-6f559d63103f', '04c40fc3-4c99-497d-945e-aa1f6c483549', 'j7ORoSQmLcAfxE1cqYculkYGCKd+Xw2ts1PBfqdDBJN5igRpoqkvsngjy9XE93HPDVvKkuX2Kc/i9BBHyTz8+Q==', '2025-08-25 19:39:24.540166', '2025-08-18 19:39:24.540165');
INSERT INTO auth.sessions VALUES ('3c2b4932-b604-478c-8bd6-17e31ef7f1b8', 'fdba92ec-89df-467f-b9c6-b0e5a13395fe', 'oilSUv3d9kBToU7NF/FKQOAXTg9N6Utq5DXqJ+1NUbEHUjof/QOf6VHUEgEJILJuvmO/emyQEK1xduQXaEV4JQ==', '2025-08-25 19:40:13.498497', '2025-08-18 19:40:13.498497');


--
-- TOC entry 3842 (class 0 OID 16962)
-- Dependencies: 253
-- Data for Name: user_sessions; Type: TABLE DATA; Schema: auth; Owner: test
--



--
-- TOC entry 3817 (class 0 OID 16444)
-- Dependencies: 228
-- Data for Name: users; Type: TABLE DATA; Schema: auth; Owner: test
--

INSERT INTO auth.users VALUES ('550e8400-e29b-41d4-a716-446655440002', 'teacher@test.com', '$2a$06$C52oJ4YxUdxdX1GHCRBPQeInBcEqHgqqJi0bM73zyJ66uo8lW4y5y', 4, true, NULL, '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502', NULL, false);
INSERT INTO auth.users VALUES ('550e8400-e29b-41d4-a716-446655440001', 'admin@planbookai.com', '$2a$06$3XDO84CVcQ9UIW.9M95tEO4Oc2BKFSgV8/GtAPhR1.Tf0GNnsc4wO', 1, true, NULL, '2025-08-18 19:07:09.558502', '2025-08-18 19:08:48.780887', NULL, false);
INSERT INTO auth.users VALUES ('04c40fc3-4c99-497d-945e-aa1f6c483549', 'test@example.com', '$2a$11$9t4rL5aCKW7kseKkNAojJOTXyoh0LG3hZaeu.R9PIVqrC8B76QqBe', 4, true, '2025-08-18 19:39:24.531878', '2025-08-18 19:37:26.523269', '2025-08-18 19:39:24.534285', NULL, false);
INSERT INTO auth.users VALUES ('fdba92ec-89df-467f-b9c6-b0e5a13395fe', 'admin2@example.com', '$2a$11$eCk/ETtww5FtxQkGy9iuSO8m7EJ/CCgUjAK9Y7jODfhTEBqNkov52', 1, true, '2025-08-18 19:40:13.490649', '2025-08-18 19:40:07.972999', '2025-08-18 19:40:13.492387', NULL, false);


--
-- TOC entry 3827 (class 0 OID 16644)
-- Dependencies: 238
-- Data for Name: chu_de; Type: TABLE DATA; Schema: content; Owner: test
--

INSERT INTO content.chu_de VALUES ('c0385e82-5858-424f-a298-ef8855f1bd3d', 'Cáº¥u trÃºc nguyÃªn tá»­', 'NghiÃªn cá»©u vá» cáº¥u trÃºc vÃ  tÃ­nh cháº¥t cá»§a nguyÃªn tá»­', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');
INSERT INTO content.chu_de VALUES ('15728637-902e-425e-a03d-4eb6e8cc2397', 'Báº£ng tuáº§n hoÃ n', 'NghiÃªn cá»©u vá» báº£ng tuáº§n hoÃ n cÃ¡c nguyÃªn tá»‘ hÃ³a há»c', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');
INSERT INTO content.chu_de VALUES ('613ed40e-d9d6-47a5-907e-6f9b681bbfe6', 'LiÃªn káº¿t hÃ³a há»c', 'NghiÃªn cá»©u vá» cÃ¡c loáº¡i liÃªn káº¿t hÃ³a há»c', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');
INSERT INTO content.chu_de VALUES ('571c3e3e-02de-4809-933b-406005653681', 'Cáº¥u trÃºc nguyÃªn tá»­', 'NghiÃªn cá»©u vá» cáº¥u trÃºc vÃ  tÃ­nh cháº¥t cá»§a nguyÃªn tá»­', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-18 19:38:41.837025', '2025-08-18 19:38:41.837025');
INSERT INTO content.chu_de VALUES ('4791d8cf-2002-445f-b82d-fb7f808b6dc7', 'Báº£ng tuáº§n hoÃ n', 'NghiÃªn cá»©u vá» báº£ng tuáº§n hoÃ n cÃ¡c nguyÃªn tá»‘ hÃ³a há»c', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-18 19:38:41.837025', '2025-08-18 19:38:41.837025');
INSERT INTO content.chu_de VALUES ('53b4fd86-f700-474f-aca4-b9fdc36cd2a2', 'LiÃªn káº¿t hÃ³a há»c', 'NghiÃªn cá»©u vá» cÃ¡c loáº¡i liÃªn káº¿t hÃ³a há»c', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-18 19:38:41.837025', '2025-08-18 19:38:41.837025');
INSERT INTO content.chu_de VALUES ('f1554986-8f3e-4c06-9888-5faa2cde686a', 'Cáº¥u trÃºc nguyÃªn tá»­', 'NghiÃªn cá»©u vá» cáº¥u trÃºc vÃ  tÃ­nh cháº¥t cá»§a nguyÃªn tá»­', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-18 19:43:34.099182', '2025-08-18 19:43:34.099182');
INSERT INTO content.chu_de VALUES ('534e957a-77ce-4c83-b101-cdc8cd4f0914', 'Báº£ng tuáº§n hoÃ n', 'NghiÃªn cá»©u vá» báº£ng tuáº§n hoÃ n cÃ¡c nguyÃªn tá»‘ hÃ³a há»c', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-18 19:43:34.099182', '2025-08-18 19:43:34.099182');
INSERT INTO content.chu_de VALUES ('43b779e8-2274-4bc0-8382-b8c0b70392f5', 'LiÃªn káº¿t hÃ³a há»c', 'NghiÃªn cá»©u vá» cÃ¡c loáº¡i liÃªn káº¿t hÃ³a há»c', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-08-18 19:43:34.099182', '2025-08-18 19:43:34.099182');


--
-- TOC entry 3826 (class 0 OID 16620)
-- Dependencies: 237
-- Data for Name: lesson_plans; Type: TABLE DATA; Schema: content; Owner: test
--



--
-- TOC entry 3825 (class 0 OID 16601)
-- Dependencies: 236
-- Data for Name: lesson_templates; Type: TABLE DATA; Schema: content; Owner: test
--



--
-- TOC entry 3833 (class 0 OID 16781)
-- Dependencies: 244
-- Data for Name: file_metadata; Type: TABLE DATA; Schema: files; Owner: test
--



--
-- TOC entry 3832 (class 0 OID 16763)
-- Dependencies: 243
-- Data for Name: file_storage; Type: TABLE DATA; Schema: files; Owner: test
--



--
-- TOC entry 3837 (class 0 OID 16849)
-- Dependencies: 248
-- Data for Name: performance_metrics; Type: TABLE DATA; Schema: logging; Owner: test
--



--
-- TOC entry 3836 (class 0 OID 16834)
-- Dependencies: 247
-- Data for Name: system_logs; Type: TABLE DATA; Schema: logging; Owner: test
--



--
-- TOC entry 3835 (class 0 OID 16818)
-- Dependencies: 246
-- Data for Name: email_queue; Type: TABLE DATA; Schema: notifications; Owner: test
--



--
-- TOC entry 3834 (class 0 OID 16801)
-- Dependencies: 245
-- Data for Name: notifications; Type: TABLE DATA; Schema: notifications; Owner: test
--



--
-- TOC entry 3831 (class 0 OID 16734)
-- Dependencies: 242
-- Data for Name: answer_sheets; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3828 (class 0 OID 16671)
-- Dependencies: 239
-- Data for Name: classes; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3830 (class 0 OID 16710)
-- Dependencies: 241
-- Data for Name: student_results; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3829 (class 0 OID 16688)
-- Dependencies: 240
-- Data for Name: students; Type: TABLE DATA; Schema: students; Owner: test
--



--
-- TOC entry 3820 (class 0 OID 16497)
-- Dependencies: 231
-- Data for Name: activity_logs; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3838 (class 0 OID 16878)
-- Dependencies: 249
-- Data for Name: otp_codes; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3839 (class 0 OID 16894)
-- Dependencies: 250
-- Data for Name: password_history; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3819 (class 0 OID 16482)
-- Dependencies: 230
-- Data for Name: user_profiles; Type: TABLE DATA; Schema: users; Owner: test
--

INSERT INTO users.user_profiles VALUES ('b747ad69-b1ba-4c10-b2de-95a35dd75914', '550e8400-e29b-41d4-a716-446655440001', 'Admin System', '0123456789', 'HÃ  Ná»™i', 'Quáº£n trá»‹ viÃªn há»‡ thá»‘ng', NULL, '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');
INSERT INTO users.user_profiles VALUES ('f5f004ea-1ea9-496b-a2a8-ccf589887813', '550e8400-e29b-41d4-a716-446655440002', 'GiÃ¡o viÃªn Test', '0987654321', 'TP.HCM', 'GiÃ¡o viÃªn HÃ³a há»c THPT', NULL, '2025-08-18 19:07:09.558502', '2025-08-18 19:07:09.558502');
INSERT INTO users.user_profiles VALUES ('62e3a1c0-deeb-44d8-8e9b-e6dc7d508750', '550e8400-e29b-41d4-a716-446655440001', 'Admin System', '0123456789', 'HÃ  Ná»™i', 'Quáº£n trá»‹ viÃªn há»‡ thá»‘ng', NULL, '2025-08-18 19:38:41.837025', '2025-08-18 19:38:41.837025');
INSERT INTO users.user_profiles VALUES ('83d24d2d-12a4-4036-aa10-d376da2458cd', '550e8400-e29b-41d4-a716-446655440002', 'GiÃ¡o viÃªn Test', '0987654321', 'TP.HCM', 'GiÃ¡o viÃªn HÃ³a há»c THPT', NULL, '2025-08-18 19:38:41.837025', '2025-08-18 19:38:41.837025');
INSERT INTO users.user_profiles VALUES ('52e61946-4d84-4e3f-8a9c-99e842765ed0', '550e8400-e29b-41d4-a716-446655440001', 'Admin System', '0123456789', 'HÃ  Ná»™i', 'Quáº£n trá»‹ viÃªn há»‡ thá»‘ng', NULL, '2025-08-18 19:43:34.099182', '2025-08-18 19:43:34.099182');
INSERT INTO users.user_profiles VALUES ('65b8a30c-ac78-4bbe-9f41-4ded02a52a82', '550e8400-e29b-41d4-a716-446655440002', 'GiÃ¡o viÃªn Test', '0987654321', 'TP.HCM', 'GiÃ¡o viÃªn HÃ³a há»c THPT', NULL, '2025-08-18 19:43:34.099182', '2025-08-18 19:43:34.099182');


--
-- TOC entry 3840 (class 0 OID 16913)
-- Dependencies: 251
-- Data for Name: user_sessions; Type: TABLE DATA; Schema: users; Owner: test
--



--
-- TOC entry 3852 (class 0 OID 0)
-- Dependencies: 226
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: auth; Owner: test
--

SELECT pg_catalog.setval('auth.roles_id_seq', 1, false);


--
-- TOC entry 3534 (class 2606 OID 16583)
-- Name: exam_questions exam_questions_exam_id_question_id_key; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_question_id_key UNIQUE (exam_id, question_id);


--
-- TOC entry 3536 (class 2606 OID 16581)
-- Name: exam_questions exam_questions_exam_id_question_order_key; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_question_order_key UNIQUE (exam_id, question_order);


--
-- TOC entry 3538 (class 2606 OID 16579)
-- Name: exam_questions exam_questions_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_pkey PRIMARY KEY (id);


--
-- TOC entry 3530 (class 2606 OID 16566)
-- Name: exams exams_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_pkey PRIMARY KEY (id);


--
-- TOC entry 3526 (class 2606 OID 16543)
-- Name: question_choices question_choices_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_pkey PRIMARY KEY (id);


--
-- TOC entry 3528 (class 2606 OID 16545)
-- Name: question_choices question_choices_question_id_choice_order_key; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_question_id_choice_order_key UNIQUE (question_id, choice_order);


--
-- TOC entry 3523 (class 2606 OID 16528)
-- Name: questions questions_pkey; Type: CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.questions
    ADD CONSTRAINT questions_pkey PRIMARY KEY (id);


--
-- TOC entry 3620 (class 2606 OID 17004)
-- Name: email_verifications email_verifications_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_pkey PRIMARY KEY (id);


--
-- TOC entry 3622 (class 2606 OID 17006)
-- Name: email_verifications email_verifications_verification_token_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_verification_token_key UNIQUE (verification_token);


--
-- TOC entry 3618 (class 2606 OID 16986)
-- Name: password_history password_history_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.password_history
    ADD CONSTRAINT password_history_pkey PRIMARY KEY (id);


--
-- TOC entry 3612 (class 2606 OID 16956)
-- Name: password_resets password_resets_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.password_resets
    ADD CONSTRAINT password_resets_pkey PRIMARY KEY (id);


--
-- TOC entry 3496 (class 2606 OID 16443)
-- Name: roles roles_name_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.roles
    ADD CONSTRAINT roles_name_key UNIQUE (name);


--
-- TOC entry 3498 (class 2606 OID 16441)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- TOC entry 3508 (class 2606 OID 16470)
-- Name: sessions sessions_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3510 (class 2606 OID 16472)
-- Name: sessions sessions_token_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_token_key UNIQUE (token);


--
-- TOC entry 3614 (class 2606 OID 16972)
-- Name: user_sessions user_sessions_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3616 (class 2606 OID 16974)
-- Name: user_sessions user_sessions_session_token_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_session_token_key UNIQUE (session_token);


--
-- TOC entry 3502 (class 2606 OID 16456)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 3504 (class 2606 OID 16454)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 3547 (class 2606 OID 16655)
-- Name: chu_de chu_de_pkey; Type: CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_pkey PRIMARY KEY (id);


--
-- TOC entry 3545 (class 2606 OID 16633)
-- Name: lesson_plans lesson_plans_pkey; Type: CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_pkey PRIMARY KEY (id);


--
-- TOC entry 3541 (class 2606 OID 16614)
-- Name: lesson_templates lesson_templates_pkey; Type: CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_templates
    ADD CONSTRAINT lesson_templates_pkey PRIMARY KEY (id);


--
-- TOC entry 3576 (class 2606 OID 16791)
-- Name: file_metadata file_metadata_file_id_key_key; Type: CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_file_id_key_key UNIQUE (file_id, key);


--
-- TOC entry 3578 (class 2606 OID 16789)
-- Name: file_metadata file_metadata_pkey; Type: CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_pkey PRIMARY KEY (id);


--
-- TOC entry 3571 (class 2606 OID 16775)
-- Name: file_storage file_storage_pkey; Type: CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_storage
    ADD CONSTRAINT file_storage_pkey PRIMARY KEY (id);


--
-- TOC entry 3595 (class 2606 OID 16855)
-- Name: performance_metrics performance_metrics_pkey; Type: CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.performance_metrics
    ADD CONSTRAINT performance_metrics_pkey PRIMARY KEY (id);


--
-- TOC entry 3591 (class 2606 OID 16843)
-- Name: system_logs system_logs_pkey; Type: CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.system_logs
    ADD CONSTRAINT system_logs_pkey PRIMARY KEY (id);


--
-- TOC entry 3586 (class 2606 OID 16829)
-- Name: email_queue email_queue_pkey; Type: CONSTRAINT; Schema: notifications; Owner: test
--

ALTER TABLE ONLY notifications.email_queue
    ADD CONSTRAINT email_queue_pkey PRIMARY KEY (id);


--
-- TOC entry 3584 (class 2606 OID 16812)
-- Name: notifications notifications_pkey; Type: CONSTRAINT; Schema: notifications; Owner: test
--

ALTER TABLE ONLY notifications.notifications
    ADD CONSTRAINT notifications_pkey PRIMARY KEY (id);


--
-- TOC entry 3567 (class 2606 OID 16744)
-- Name: answer_sheets answer_sheets_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_pkey PRIMARY KEY (id);


--
-- TOC entry 3551 (class 2606 OID 16682)
-- Name: classes classes_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.classes
    ADD CONSTRAINT classes_pkey PRIMARY KEY (id);


--
-- TOC entry 3563 (class 2606 OID 16721)
-- Name: student_results student_results_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_pkey PRIMARY KEY (id);


--
-- TOC entry 3565 (class 2606 OID 16723)
-- Name: student_results student_results_student_id_exam_id_key; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_student_id_exam_id_key UNIQUE (student_id, exam_id);


--
-- TOC entry 3557 (class 2606 OID 16699)
-- Name: students students_owner_teacher_id_student_code_key; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_owner_teacher_id_student_code_key UNIQUE (owner_teacher_id, student_code);


--
-- TOC entry 3559 (class 2606 OID 16697)
-- Name: students students_pkey; Type: CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_pkey PRIMARY KEY (id);


--
-- TOC entry 3515 (class 2606 OID 16505)
-- Name: activity_logs activity_logs_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.activity_logs
    ADD CONSTRAINT activity_logs_pkey PRIMARY KEY (id);


--
-- TOC entry 3600 (class 2606 OID 16888)
-- Name: otp_codes otp_codes_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.otp_codes
    ADD CONSTRAINT otp_codes_pkey PRIMARY KEY (id);


--
-- TOC entry 3603 (class 2606 OID 16902)
-- Name: password_history password_history_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_pkey PRIMARY KEY (id);


--
-- TOC entry 3513 (class 2606 OID 16491)
-- Name: user_profiles user_profiles_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_profiles
    ADD CONSTRAINT user_profiles_pkey PRIMARY KEY (id);


--
-- TOC entry 3608 (class 2606 OID 16923)
-- Name: user_sessions user_sessions_pkey; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_pkey PRIMARY KEY (id);


--
-- TOC entry 3610 (class 2606 OID 16925)
-- Name: user_sessions user_sessions_session_token_key; Type: CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_session_token_key UNIQUE (session_token);


--
-- TOC entry 3531 (class 1259 OID 16599)
-- Name: idx_assessment_exams_subject; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_exams_subject ON assessment.exams USING btree (subject);


--
-- TOC entry 3532 (class 1259 OID 16598)
-- Name: idx_assessment_exams_teacher; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_exams_teacher ON assessment.exams USING btree (teacher_id);


--
-- TOC entry 3524 (class 1259 OID 16600)
-- Name: idx_assessment_question_choices_question; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_question_choices_question ON assessment.question_choices USING btree (question_id);


--
-- TOC entry 3518 (class 1259 OID 16597)
-- Name: idx_assessment_questions_created_by; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_created_by ON assessment.questions USING btree (created_by);


--
-- TOC entry 3519 (class 1259 OID 16596)
-- Name: idx_assessment_questions_difficulty; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_difficulty ON assessment.questions USING btree (difficulty);


--
-- TOC entry 3520 (class 1259 OID 16594)
-- Name: idx_assessment_questions_subject; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_subject ON assessment.questions USING btree (subject);


--
-- TOC entry 3521 (class 1259 OID 16595)
-- Name: idx_assessment_questions_topic; Type: INDEX; Schema: assessment; Owner: test
--

CREATE INDEX idx_assessment_questions_topic ON assessment.questions USING btree (topic);


--
-- TOC entry 3505 (class 1259 OID 16481)
-- Name: idx_auth_sessions_token; Type: INDEX; Schema: auth; Owner: test
--

CREATE INDEX idx_auth_sessions_token ON auth.sessions USING btree (token);


--
-- TOC entry 3506 (class 1259 OID 16480)
-- Name: idx_auth_sessions_user; Type: INDEX; Schema: auth; Owner: test
--

CREATE INDEX idx_auth_sessions_user ON auth.sessions USING btree (user_id);


--
-- TOC entry 3499 (class 1259 OID 16478)
-- Name: idx_auth_users_email; Type: INDEX; Schema: auth; Owner: test
--

CREATE INDEX idx_auth_users_email ON auth.users USING btree (email);


--
-- TOC entry 3500 (class 1259 OID 16479)
-- Name: idx_auth_users_role; Type: INDEX; Schema: auth; Owner: test
--

CREATE INDEX idx_auth_users_role ON auth.users USING btree (role_id);


--
-- TOC entry 3548 (class 1259 OID 16670)
-- Name: idx_content_chu_de_parent; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_chu_de_parent ON content.chu_de USING btree (parent_id);


--
-- TOC entry 3549 (class 1259 OID 16669)
-- Name: idx_content_chu_de_subject; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_chu_de_subject ON content.chu_de USING btree (subject);


--
-- TOC entry 3542 (class 1259 OID 16668)
-- Name: idx_content_lesson_plans_subject; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_lesson_plans_subject ON content.lesson_plans USING btree (subject);


--
-- TOC entry 3543 (class 1259 OID 16667)
-- Name: idx_content_lesson_plans_teacher; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_lesson_plans_teacher ON content.lesson_plans USING btree (teacher_id);


--
-- TOC entry 3539 (class 1259 OID 16666)
-- Name: idx_content_lesson_templates_subject; Type: INDEX; Schema: content; Owner: test
--

CREATE INDEX idx_content_lesson_templates_subject ON content.lesson_templates USING btree (subject);


--
-- TOC entry 3579 (class 1259 OID 16800)
-- Name: idx_files_metadata_file; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_metadata_file ON files.file_metadata USING btree (file_id);


--
-- TOC entry 3572 (class 1259 OID 16798)
-- Name: idx_files_storage_file_type; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_storage_file_type ON files.file_storage USING btree (file_type);


--
-- TOC entry 3573 (class 1259 OID 16799)
-- Name: idx_files_storage_status; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_storage_status ON files.file_storage USING btree (status);


--
-- TOC entry 3574 (class 1259 OID 16797)
-- Name: idx_files_storage_uploaded_by; Type: INDEX; Schema: files; Owner: test
--

CREATE INDEX idx_files_storage_uploaded_by ON files.file_storage USING btree (uploaded_by);


--
-- TOC entry 3592 (class 1259 OID 16864)
-- Name: idx_logging_performance_created_at; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_performance_created_at ON logging.performance_metrics USING btree (created_at);


--
-- TOC entry 3593 (class 1259 OID 16863)
-- Name: idx_logging_performance_service; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_performance_service ON logging.performance_metrics USING btree (service_name);


--
-- TOC entry 3588 (class 1259 OID 16862)
-- Name: idx_logging_system_logs_created_at; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_system_logs_created_at ON logging.system_logs USING btree (created_at);


--
-- TOC entry 3589 (class 1259 OID 16861)
-- Name: idx_logging_system_logs_level; Type: INDEX; Schema: logging; Owner: test
--

CREATE INDEX idx_logging_system_logs_level ON logging.system_logs USING btree (level);


--
-- TOC entry 3587 (class 1259 OID 16833)
-- Name: idx_email_queue_status; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_email_queue_status ON notifications.email_queue USING btree (status);


--
-- TOC entry 3580 (class 1259 OID 16832)
-- Name: idx_notifications_created_at; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_notifications_created_at ON notifications.notifications USING btree (created_at);


--
-- TOC entry 3581 (class 1259 OID 16831)
-- Name: idx_notifications_is_read; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_notifications_is_read ON notifications.notifications USING btree (is_read);


--
-- TOC entry 3582 (class 1259 OID 16830)
-- Name: idx_notifications_user; Type: INDEX; Schema: notifications; Owner: test
--

CREATE INDEX idx_notifications_user ON notifications.notifications USING btree (user_id);


--
-- TOC entry 3568 (class 1259 OID 16762)
-- Name: idx_students_answer_sheets_exam; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_answer_sheets_exam ON students.answer_sheets USING btree (exam_id);


--
-- TOC entry 3569 (class 1259 OID 16761)
-- Name: idx_students_answer_sheets_student; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_answer_sheets_student ON students.answer_sheets USING btree (student_id);


--
-- TOC entry 3552 (class 1259 OID 16755)
-- Name: idx_students_classes_teacher; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_classes_teacher ON students.classes USING btree (homeroom_teacher_id);


--
-- TOC entry 3560 (class 1259 OID 16760)
-- Name: idx_students_student_results_exam; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_student_results_exam ON students.student_results USING btree (exam_id);


--
-- TOC entry 3561 (class 1259 OID 16759)
-- Name: idx_students_student_results_student; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_student_results_student ON students.student_results USING btree (student_id);


--
-- TOC entry 3553 (class 1259 OID 16756)
-- Name: idx_students_students_class; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_students_class ON students.students USING btree (class_id);


--
-- TOC entry 3554 (class 1259 OID 16758)
-- Name: idx_students_students_code; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_students_code ON students.students USING btree (student_code);


--
-- TOC entry 3555 (class 1259 OID 16757)
-- Name: idx_students_students_teacher; Type: INDEX; Schema: students; Owner: test
--

CREATE INDEX idx_students_students_teacher ON students.students USING btree (owner_teacher_id);


--
-- TOC entry 3516 (class 1259 OID 16513)
-- Name: idx_users_activity_logs_created; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_activity_logs_created ON users.activity_logs USING btree (created_at);


--
-- TOC entry 3517 (class 1259 OID 16512)
-- Name: idx_users_activity_logs_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_activity_logs_user ON users.activity_logs USING btree (user_id);


--
-- TOC entry 3596 (class 1259 OID 16933)
-- Name: idx_users_otp_codes_expires; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_otp_codes_expires ON users.otp_codes USING btree (expires_at);


--
-- TOC entry 3597 (class 1259 OID 16932)
-- Name: idx_users_otp_codes_purpose; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_otp_codes_purpose ON users.otp_codes USING btree (purpose);


--
-- TOC entry 3598 (class 1259 OID 16931)
-- Name: idx_users_otp_codes_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_otp_codes_user ON users.otp_codes USING btree (user_id);


--
-- TOC entry 3601 (class 1259 OID 16934)
-- Name: idx_users_password_history_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_password_history_user ON users.password_history USING btree (user_id);


--
-- TOC entry 3511 (class 1259 OID 16511)
-- Name: idx_users_profiles_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_profiles_user ON users.user_profiles USING btree (user_id);


--
-- TOC entry 3604 (class 1259 OID 16937)
-- Name: idx_users_user_sessions_active; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_user_sessions_active ON users.user_sessions USING btree (is_active);


--
-- TOC entry 3605 (class 1259 OID 16936)
-- Name: idx_users_user_sessions_token; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_user_sessions_token ON users.user_sessions USING btree (session_token);


--
-- TOC entry 3606 (class 1259 OID 16935)
-- Name: idx_users_user_sessions_user; Type: INDEX; Schema: users; Owner: test
--

CREATE INDEX idx_users_user_sessions_user ON users.user_sessions USING btree (user_id);


--
-- TOC entry 3663 (class 2620 OID 16870)
-- Name: exams trigger_updated_at_assessment_exams; Type: TRIGGER; Schema: assessment; Owner: test
--

CREATE TRIGGER trigger_updated_at_assessment_exams BEFORE UPDATE ON assessment.exams FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3662 (class 2620 OID 16869)
-- Name: questions trigger_updated_at_assessment_questions; Type: TRIGGER; Schema: assessment; Owner: test
--

CREATE TRIGGER trigger_updated_at_assessment_questions BEFORE UPDATE ON assessment.questions FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3658 (class 2620 OID 16866)
-- Name: roles trigger_updated_at_auth_roles; Type: TRIGGER; Schema: auth; Owner: test
--

CREATE TRIGGER trigger_updated_at_auth_roles BEFORE UPDATE ON auth.roles FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3659 (class 2620 OID 16867)
-- Name: users trigger_updated_at_auth_users; Type: TRIGGER; Schema: auth; Owner: test
--

CREATE TRIGGER trigger_updated_at_auth_users BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3660 (class 2620 OID 16947)
-- Name: users trigger_updated_at_auth_users_soft_delete; Type: TRIGGER; Schema: auth; Owner: test
--

CREATE TRIGGER trigger_updated_at_auth_users_soft_delete BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3666 (class 2620 OID 16873)
-- Name: chu_de trigger_updated_at_content_chu_de; Type: TRIGGER; Schema: content; Owner: test
--

CREATE TRIGGER trigger_updated_at_content_chu_de BEFORE UPDATE ON content.chu_de FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3665 (class 2620 OID 16872)
-- Name: lesson_plans trigger_updated_at_content_lesson_plans; Type: TRIGGER; Schema: content; Owner: test
--

CREATE TRIGGER trigger_updated_at_content_lesson_plans BEFORE UPDATE ON content.lesson_plans FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3664 (class 2620 OID 16871)
-- Name: lesson_templates trigger_updated_at_content_lesson_templates; Type: TRIGGER; Schema: content; Owner: test
--

CREATE TRIGGER trigger_updated_at_content_lesson_templates BEFORE UPDATE ON content.lesson_templates FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3669 (class 2620 OID 16876)
-- Name: file_storage trigger_updated_at_files_file_storage; Type: TRIGGER; Schema: files; Owner: test
--

CREATE TRIGGER trigger_updated_at_files_file_storage BEFORE UPDATE ON files.file_storage FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3667 (class 2620 OID 16874)
-- Name: classes trigger_updated_at_students_classes; Type: TRIGGER; Schema: students; Owner: test
--

CREATE TRIGGER trigger_updated_at_students_classes BEFORE UPDATE ON students.classes FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3668 (class 2620 OID 16875)
-- Name: students trigger_updated_at_students_students; Type: TRIGGER; Schema: students; Owner: test
--

CREATE TRIGGER trigger_updated_at_students_students BEFORE UPDATE ON students.students FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3661 (class 2620 OID 16868)
-- Name: user_profiles trigger_updated_at_users_user_profiles; Type: TRIGGER; Schema: users; Owner: test
--

CREATE TRIGGER trigger_updated_at_users_user_profiles BEFORE UPDATE ON users.user_profiles FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- TOC entry 3630 (class 2606 OID 16584)
-- Name: exam_questions exam_questions_exam_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id) ON DELETE CASCADE;


--
-- TOC entry 3631 (class 2606 OID 16589)
-- Name: exam_questions exam_questions_question_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_question_id_fkey FOREIGN KEY (question_id) REFERENCES assessment.questions(id);


--
-- TOC entry 3629 (class 2606 OID 16567)
-- Name: exams exams_teacher_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3628 (class 2606 OID 16546)
-- Name: question_choices question_choices_question_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_question_id_fkey FOREIGN KEY (question_id) REFERENCES assessment.questions(id) ON DELETE CASCADE;


--
-- TOC entry 3627 (class 2606 OID 16529)
-- Name: questions questions_created_by_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: test
--

ALTER TABLE ONLY assessment.questions
    ADD CONSTRAINT questions_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3657 (class 2606 OID 17007)
-- Name: email_verifications email_verifications_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3655 (class 2606 OID 16992)
-- Name: password_history password_history_changed_by_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.password_history
    ADD CONSTRAINT password_history_changed_by_fkey FOREIGN KEY (changed_by) REFERENCES auth.users(id);


--
-- TOC entry 3656 (class 2606 OID 16987)
-- Name: password_history password_history_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.password_history
    ADD CONSTRAINT password_history_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3653 (class 2606 OID 16957)
-- Name: password_resets password_resets_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.password_resets
    ADD CONSTRAINT password_resets_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3624 (class 2606 OID 16473)
-- Name: sessions sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3654 (class 2606 OID 16975)
-- Name: user_sessions user_sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3623 (class 2606 OID 16457)
-- Name: users users_role_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: test
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES auth.roles(id);


--
-- TOC entry 3635 (class 2606 OID 16661)
-- Name: chu_de chu_de_created_by_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3636 (class 2606 OID 16656)
-- Name: chu_de chu_de_parent_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_parent_id_fkey FOREIGN KEY (parent_id) REFERENCES content.chu_de(id);


--
-- TOC entry 3633 (class 2606 OID 16634)
-- Name: lesson_plans lesson_plans_teacher_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3634 (class 2606 OID 16639)
-- Name: lesson_plans lesson_plans_template_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_template_id_fkey FOREIGN KEY (template_id) REFERENCES content.lesson_templates(id);


--
-- TOC entry 3632 (class 2606 OID 16615)
-- Name: lesson_templates lesson_templates_created_by_fkey; Type: FK CONSTRAINT; Schema: content; Owner: test
--

ALTER TABLE ONLY content.lesson_templates
    ADD CONSTRAINT lesson_templates_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- TOC entry 3645 (class 2606 OID 16792)
-- Name: file_metadata file_metadata_file_id_fkey; Type: FK CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_file_id_fkey FOREIGN KEY (file_id) REFERENCES files.file_storage(id) ON DELETE CASCADE;


--
-- TOC entry 3644 (class 2606 OID 16776)
-- Name: file_storage file_storage_uploaded_by_fkey; Type: FK CONSTRAINT; Schema: files; Owner: test
--

ALTER TABLE ONLY files.file_storage
    ADD CONSTRAINT file_storage_uploaded_by_fkey FOREIGN KEY (uploaded_by) REFERENCES auth.users(id);


--
-- TOC entry 3648 (class 2606 OID 16856)
-- Name: performance_metrics performance_metrics_user_id_fkey; Type: FK CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.performance_metrics
    ADD CONSTRAINT performance_metrics_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3647 (class 2606 OID 16844)
-- Name: system_logs system_logs_user_id_fkey; Type: FK CONSTRAINT; Schema: logging; Owner: test
--

ALTER TABLE ONLY logging.system_logs
    ADD CONSTRAINT system_logs_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3646 (class 2606 OID 16813)
-- Name: notifications notifications_user_id_fkey; Type: FK CONSTRAINT; Schema: notifications; Owner: test
--

ALTER TABLE ONLY notifications.notifications
    ADD CONSTRAINT notifications_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- TOC entry 3642 (class 2606 OID 16750)
-- Name: answer_sheets answer_sheets_exam_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id);


--
-- TOC entry 3643 (class 2606 OID 16745)
-- Name: answer_sheets answer_sheets_student_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_student_id_fkey FOREIGN KEY (student_id) REFERENCES students.students(id);


--
-- TOC entry 3637 (class 2606 OID 16683)
-- Name: classes classes_homeroom_teacher_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.classes
    ADD CONSTRAINT classes_homeroom_teacher_id_fkey FOREIGN KEY (homeroom_teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3640 (class 2606 OID 16729)
-- Name: student_results student_results_exam_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id);


--
-- TOC entry 3641 (class 2606 OID 16724)
-- Name: student_results student_results_student_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_student_id_fkey FOREIGN KEY (student_id) REFERENCES students.students(id);


--
-- TOC entry 3638 (class 2606 OID 16700)
-- Name: students students_class_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_class_id_fkey FOREIGN KEY (class_id) REFERENCES students.classes(id);


--
-- TOC entry 3639 (class 2606 OID 16705)
-- Name: students students_owner_teacher_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: test
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_owner_teacher_id_fkey FOREIGN KEY (owner_teacher_id) REFERENCES auth.users(id);


--
-- TOC entry 3626 (class 2606 OID 16506)
-- Name: activity_logs activity_logs_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.activity_logs
    ADD CONSTRAINT activity_logs_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3649 (class 2606 OID 16889)
-- Name: otp_codes otp_codes_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.otp_codes
    ADD CONSTRAINT otp_codes_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3650 (class 2606 OID 16908)
-- Name: password_history password_history_changed_by_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_changed_by_fkey FOREIGN KEY (changed_by) REFERENCES auth.users(id);


--
-- TOC entry 3651 (class 2606 OID 16903)
-- Name: password_history password_history_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3625 (class 2606 OID 16492)
-- Name: user_profiles user_profiles_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_profiles
    ADD CONSTRAINT user_profiles_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- TOC entry 3652 (class 2606 OID 16926)
-- Name: user_sessions user_sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: test
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


-- Completed on 2025-08-19 03:14:29

--
-- PostgreSQL database dump complete
--

