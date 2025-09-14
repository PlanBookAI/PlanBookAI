--
-- PostgreSQL database dump
--

\restrict NAmfK93R50azqc8k9PPgWFLJQ4TcK8JZz0VZSwwprwzDeSHqE7cgLycLfZQbkRR

-- Dumped from database version 17.6 (Debian 17.6-1.pgdg13+1)
-- Dumped by pg_dump version 17.6 (Debian 17.6-1.pgdg13+1)

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
-- Name: planbookai; Type: DATABASE; Schema: -; Owner: -
--

CREATE DATABASE planbookai WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


\unrestrict NAmfK93R50azqc8k9PPgWFLJQ4TcK8JZz0VZSwwprwzDeSHqE7cgLycLfZQbkRR
\connect planbookai
\restrict NAmfK93R50azqc8k9PPgWFLJQ4TcK8JZz0VZSwwprwzDeSHqE7cgLycLfZQbkRR

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
-- Name: assessment; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA assessment;


--
-- Name: auth; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA auth;


--
-- Name: content; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA content;


--
-- Name: files; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA files;


--
-- Name: logging; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA logging;


--
-- Name: notifications; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA notifications;


--
-- Name: students; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA students;


--
-- Name: users; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA users;


--
-- Name: update_updated_at_column(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.update_updated_at_column() RETURNS trigger
    LANGUAGE plpgsql
    AS $$ BEGIN NEW.updated_at = CURRENT_TIMESTAMP; RETURN NEW; END; $$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
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
-- Name: exam_templates; Type: TABLE; Schema: assessment; Owner: -
--

CREATE TABLE assessment.exam_templates (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    title character varying(255) NOT NULL,
    description text,
    subject character varying(100) DEFAULT 'HOA_HOC'::character varying NOT NULL,
    grade integer,
    duration_minutes integer,
    total_score numeric(5,2),
    structure jsonb,
    created_by uuid NOT NULL,
    status character varying(50) DEFAULT 'ACTIVE'::character varying,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT exam_templates_duration_minutes_check CHECK ((duration_minutes > 0)),
    CONSTRAINT exam_templates_grade_check CHECK ((grade = ANY (ARRAY[10, 11, 12]))),
    CONSTRAINT exam_templates_status_check CHECK (((status)::text = ANY (ARRAY[('ACTIVE'::character varying)::text, ('INACTIVE'::character varying)::text, ('ARCHIVED'::character varying)::text]))),
    CONSTRAINT exam_templates_total_score_check CHECK ((total_score > (0)::numeric))
);


--
-- Name: TABLE exam_templates; Type: COMMENT; Schema: assessment; Owner: -
--

COMMENT ON TABLE assessment.exam_templates IS 'Bảng lưu trữ các mẫu đề thi để tái sử dụng';


--
-- Name: COLUMN exam_templates.structure; Type: COMMENT; Schema: assessment; Owner: -
--

COMMENT ON COLUMN assessment.exam_templates.structure IS 'Cấu trúc đề thi dạng JSON: {"easy": 5, "medium": 10, "hard": 3, "topics": ["topic1", "topic2"]}';


--
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
    template_id uuid,
    CONSTRAINT exams_duration_minutes_check CHECK ((duration_minutes > 0)),
    CONSTRAINT exams_grade_check CHECK ((grade = ANY (ARRAY[10, 11, 12]))),
    CONSTRAINT exams_status_check CHECK (((status)::text = ANY (ARRAY[('DRAFT'::character varying)::text, ('PUBLISHED'::character varying)::text, ('COMPLETED'::character varying)::text, ('ARCHIVED'::character varying)::text]))),
    CONSTRAINT exams_total_score_check CHECK ((total_score > (0)::numeric))
);


--
-- Name: COLUMN exams.template_id; Type: COMMENT; Schema: assessment; Owner: -
--

COMMENT ON COLUMN assessment.exams.template_id IS 'ID của mẫu đề thi được sử dụng để tạo đề thi này';


--
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
    points numeric(5,2) DEFAULT 1.0,
    CONSTRAINT questions_difficulty_check CHECK (((difficulty)::text = ANY (ARRAY[('EASY'::character varying)::text, ('MEDIUM'::character varying)::text, ('HARD'::character varying)::text, ('VERY_HARD'::character varying)::text]))),
    CONSTRAINT questions_status_check CHECK (((status)::text = ANY (ARRAY[('ACTIVE'::character varying)::text, ('INACTIVE'::character varying)::text, ('ARCHIVED'::character varying)::text]))),
    CONSTRAINT questions_type_check CHECK (((type)::text = ANY (ARRAY[('MULTIPLE_CHOICE'::character varying)::text, ('ESSAY'::character varying)::text, ('SHORT_ANSWER'::character varying)::text, ('TRUE_FALSE'::character varying)::text])))
);


--
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
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: auth; Owner: -
--

ALTER SEQUENCE auth.roles_id_seq OWNED BY auth.roles.id;


--
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
    CONSTRAINT lesson_plans_status_check CHECK (((status)::text = ANY (ARRAY[('DRAFT'::character varying)::text, ('COMPLETED'::character varying)::text, ('PUBLISHED'::character varying)::text, ('ARCHIVED'::character varying)::text])))
);


--
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
    CONSTRAINT lesson_templates_status_check CHECK (((status)::text = ANY (ARRAY[('ACTIVE'::character varying)::text, ('INACTIVE'::character varying)::text, ('ARCHIVED'::character varying)::text])))
);


--
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
    CONSTRAINT file_storage_file_type_check CHECK (((file_type)::text = ANY (ARRAY[('IMAGE'::character varying)::text, ('DOCUMENT'::character varying)::text, ('PDF'::character varying)::text, ('EXCEL'::character varying)::text, ('OTHER'::character varying)::text]))),
    CONSTRAINT file_storage_status_check CHECK (((status)::text = ANY (ARRAY[('ACTIVE'::character varying)::text, ('ARCHIVED'::character varying)::text, ('DELETED'::character varying)::text])))
);


--
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
    CONSTRAINT system_logs_level_check CHECK (((level)::text = ANY (ARRAY[('DEBUG'::character varying)::text, ('INFO'::character varying)::text, ('WARNING'::character varying)::text, ('ERROR'::character varying)::text, ('CRITICAL'::character varying)::text])))
);


--
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
    CONSTRAINT email_queue_status_check CHECK (((status)::text = ANY (ARRAY[('PENDING'::character varying)::text, ('SENT'::character varying)::text, ('FAILED'::character varying)::text])))
);


--
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
    CONSTRAINT notifications_type_check CHECK (((type)::text = ANY (ARRAY[('INFO'::character varying)::text, ('SUCCESS'::character varying)::text, ('WARNING'::character varying)::text, ('ERROR'::character varying)::text])))
);


--
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
    CONSTRAINT answer_sheets_ocr_status_check CHECK (((ocr_status)::text = ANY (ARRAY[('PENDING'::character varying)::text, ('PROCESSING'::character varying)::text, ('COMPLETED'::character varying)::text, ('FAILED'::character varying)::text])))
);


--
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
    CONSTRAINT student_results_grading_method_check CHECK (((grading_method)::text = ANY (ARRAY[('OCR'::character varying)::text, ('MANUAL'::character varying)::text, ('AUTO'::character varying)::text]))),
    CONSTRAINT student_results_score_check CHECK ((score >= (0)::numeric))
);


--
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
    CONSTRAINT students_gender_check CHECK (((gender)::text = ANY (ARRAY[('MALE'::character varying)::text, ('FEMALE'::character varying)::text, ('OTHER'::character varying)::text])))
);


--
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
    CONSTRAINT otp_codes_purpose_check CHECK (((purpose)::text = ANY (ARRAY[('PASSWORD_RESET'::character varying)::text, ('EMAIL_VERIFICATION'::character varying)::text, ('PHONE_VERIFICATION'::character varying)::text])))
);


--
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
-- Name: roles id; Type: DEFAULT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.roles ALTER COLUMN id SET DEFAULT nextval('auth.roles_id_seq'::regclass);


--
-- Data for Name: exam_questions; Type: TABLE DATA; Schema: assessment; Owner: -
--



--
-- Data for Name: exam_templates; Type: TABLE DATA; Schema: assessment; Owner: -
--

INSERT INTO assessment.exam_templates VALUES ('8894f6cb-7497-4101-a77c-42fc72f4fdc4', 'Mẫu đề thi Hóa học lớp 10 - Cơ bản', 'Mẫu đề thi cơ bản cho học sinh lớp 10 môn Hóa học', 'HOA_HOC', 10, 45, 10.00, '{"easy": 8, "hard": 2, "medium": 6, "topics": ["Cấu trúc nguyên tử", "Bảng tuần hoàn"]}', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-14 08:19:16.161405', '2025-09-14 08:19:16.161405');
INSERT INTO assessment.exam_templates VALUES ('d10c12f7-b90a-4c2f-9567-599d290bcc0f', 'Mẫu đề thi Hóa học lớp 11 - Nâng cao', 'Mẫu đề thi nâng cao cho học sinh lớp 11 môn Hóa học', 'HOA_HOC', 11, 60, 10.00, '{"easy": 5, "hard": 4, "medium": 8, "topics": ["Liên kết hóa học", "Phản ứng hóa học"]}', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-14 08:19:16.161405', '2025-09-14 08:19:16.161405');


--
-- Data for Name: exams; Type: TABLE DATA; Schema: assessment; Owner: -
--



--
-- Data for Name: question_choices; Type: TABLE DATA; Schema: assessment; Owner: -
--

INSERT INTO assessment.question_choices VALUES ('ab099e94-7903-48c9-a755-f7ca1c7bcee0', 'c7495ba4-a288-42a6-bc5c-78163c388de8', 'A', 'Hạt nhân và electron', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices VALUES ('f7f4da56-d7b0-446b-b13a-cdda8b4414ea', 'c7495ba4-a288-42a6-bc5c-78163c388de8', 'B', 'Chỉ có hạt nhân', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices VALUES ('eea6a267-ef11-4a79-ac4c-f7d4aa585938', 'c7495ba4-a288-42a6-bc5c-78163c388de8', 'C', 'Chỉ có electron', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices VALUES ('5cb6ca66-85c0-4f1a-bc43-d64b4548fc86', 'c7495ba4-a288-42a6-bc5c-78163c388de8', 'D', 'Proton và neutron', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices VALUES ('70675195-aecf-487f-8910-ba3f3d6ec7d0', 'd2d42623-0b35-442e-841b-43e8f487f060', 'A', 'Theo khối lượng nguyên tử', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices VALUES ('bd676918-fd1d-4841-b128-2588f8fff8ad', 'd2d42623-0b35-442e-841b-43e8f487f060', 'B', 'Theo số hiệu nguyên tử tăng dần', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices VALUES ('fe013133-0e68-450d-ac0a-8764bc517995', 'd2d42623-0b35-442e-841b-43e8f487f060', 'C', 'Theo tên nguyên tố', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices VALUES ('c44d0128-a206-4de5-9d8b-c218c6d5be72', 'd2d42623-0b35-442e-841b-43e8f487f060', 'D', 'Theo màu sắc', '2025-09-12 07:40:16.04764');
INSERT INTO assessment.question_choices VALUES ('022a8315-256e-4b77-8d90-f7cd73fecb89', 'a77aa669-4623-4104-99d6-684673a578c9', 'C', 'HNO3', '2025-09-12 08:12:05.860273');
INSERT INTO assessment.question_choices VALUES ('62578e12-bfe2-4052-8a29-4916dede6152', 'a77aa669-4623-4104-99d6-684673a578c9', 'D', 'H3PO4', '2025-09-12 08:12:05.860273');
INSERT INTO assessment.question_choices VALUES ('d6f365ef-0b8d-4ba9-8cc4-3a1afa1c40c9', 'a77aa669-4623-4104-99d6-684673a578c9', 'B', 'H2SO4', '2025-09-12 08:12:05.860273');
INSERT INTO assessment.question_choices VALUES ('fd78c35a-a2d1-4c36-892d-de5a9eaa0fe3', 'a77aa669-4623-4104-99d6-684673a578c9', 'A', 'HCl', '2025-09-12 08:12:05.860205');
INSERT INTO assessment.question_choices VALUES ('19679690-3184-4410-abb0-d19d71687071', '7f314536-f6f0-41ec-9e88-2c5671897362', 'D', 'H3PO4', '2025-09-12 08:12:46.595286');
INSERT INTO assessment.question_choices VALUES ('502d6a2b-b1c2-434c-8430-7a444ededfe7', '7f314536-f6f0-41ec-9e88-2c5671897362', 'B', 'H2SO4', '2025-09-12 08:12:46.595286');
INSERT INTO assessment.question_choices VALUES ('5d39a298-6ca9-4ebf-87b1-4f4aa7308e8b', '7f314536-f6f0-41ec-9e88-2c5671897362', 'C', 'HNO3', '2025-09-12 08:12:46.595286');
INSERT INTO assessment.question_choices VALUES ('76a8d71f-934f-461f-b638-4fab6965fcaa', '7f314536-f6f0-41ec-9e88-2c5671897362', 'A', 'HCl', '2025-09-12 08:12:46.595284');


--
-- Data for Name: questions; Type: TABLE DATA; Schema: assessment; Owner: -
--

INSERT INTO assessment.questions VALUES ('c7495ba4-a288-42a6-bc5c-78163c388de8', 'Nguyên tử có cấu trúc như thế nào?', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 'Cấu trúc nguyên tử', 'A', 'Nguyên tử gồm hạt nhân và electron', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764', 1.00);
INSERT INTO assessment.questions VALUES ('d2d42623-0b35-442e-841b-43e8f487f060', 'Trong bảng tuần hoàn, các nguyên tố được sắp xếp theo:', 'MULTIPLE_CHOICE', 'MEDIUM', 'HOA_HOC', 'Bảng tuần hoàn', 'B', 'Theo số hiệu nguyên tử tăng dần', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764', 1.00);
INSERT INTO assessment.questions VALUES ('6ebffe14-1599-4ebe-894e-87241d5caeac', 'Test question', 'MULTIPLE_CHOICE', 'MEDIUM', 'HOA_HOC', NULL, NULL, NULL, '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-12 08:03:51.987265', '2025-09-12 08:03:51.987265', 1.00);
INSERT INTO assessment.questions VALUES ('a77aa669-4623-4104-99d6-684673a578c9', 'Hãy chọn công thức hóa học đúng của axit clohiđric:', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 'Axit - Bazơ - Muối', 'A', 'Axit clohiđric có công thức HCl', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-12 08:12:05.859811', '2025-09-12 08:12:05.859812', 1.00);
INSERT INTO assessment.questions VALUES ('7f314536-f6f0-41ec-9e88-2c5671897362', 'Hãy chọn công thức hóa học đúng của axit clohiđric:', 'MULTIPLE_CHOICE', 'EASY', 'HOA_HOC', 'Axit - Bazơ - Muối', 'A', 'Axit clohiđric có công thức HCl', '550e8400-e29b-41d4-a716-446655440002', 'ACTIVE', '2025-09-12 08:12:46.595278', '2025-09-12 08:12:46.595278', 1.00);


--
-- Data for Name: email_verifications; Type: TABLE DATA; Schema: auth; Owner: -
--



--
-- Data for Name: password_history; Type: TABLE DATA; Schema: auth; Owner: -
--



--
-- Data for Name: password_resets; Type: TABLE DATA; Schema: auth; Owner: -
--



--
-- Data for Name: roles; Type: TABLE DATA; Schema: auth; Owner: -
--

INSERT INTO auth.roles VALUES (1, 'ADMIN', 'Quản trị viên hệ thống', true, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO auth.roles VALUES (2, 'MANAGER', 'Quản lý nội dung và người dùng', true, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO auth.roles VALUES (3, 'STAFF', 'Nhân viên tạo nội dung mẫu', true, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO auth.roles VALUES (4, 'TEACHER', 'Giáo viên sử dụng hệ thống', true, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');


--
-- Data for Name: sessions; Type: TABLE DATA; Schema: auth; Owner: -
--

INSERT INTO auth.sessions VALUES ('67468aac-4895-4760-9ba9-b9734706de3e', '550e8400-e29b-41d4-a716-446655440002', 'P4mFhLeEK/HHWgKSJCgYg86p+PJz5bNtlVFTZB8uwgsuZdWWVmuNDXOQNquqvEtUSw8z0Pk30xjC8tU46ydXkw==', '2025-09-19 07:41:25.429243', '2025-09-12 07:41:25.429186');
INSERT INTO auth.sessions VALUES ('bf4c06e6-4b4d-4fec-b8c8-90cef0331543', '550e8400-e29b-41d4-a716-446655440002', 'HQKszHg4l+OUReqe2143zFaGBMthiIf5TPGgseViDrr5bdOkUxfD7UwvtQZ/UqBaGQlQXb/NGMS5HFuIf4w+5g==', '2025-09-19 07:41:56.986942', '2025-09-12 07:41:56.986942');
INSERT INTO auth.sessions VALUES ('a89d818e-72fe-4ae7-8073-0fa8e9a2a4ab', '550e8400-e29b-41d4-a716-446655440002', '7b49McChO7wX/cgXy7W9kvCBKJJwja45SruPvVyOzqwrkIxGzsmW7ft/eJtmwHPXC2QmePpGJikMA6svaeJ19w==', '2025-09-19 07:58:38.030585', '2025-09-12 07:58:38.030526');
INSERT INTO auth.sessions VALUES ('51bbc143-637c-4145-afce-8ddb4be39b75', '550e8400-e29b-41d4-a716-446655440002', 'jWd9BG7oRqDa9GVQNCTxeFbCaClqPvYIl+EcRe8i/RNezWxB+gR7QfdzSywev1RoY0j/4eAPBg1WSk9rzuCRjg==', '2025-09-19 08:05:44.24244', '2025-09-12 08:05:44.242344');
INSERT INTO auth.sessions VALUES ('390d157a-a0af-43db-b29f-3805e0172e0c', '550e8400-e29b-41d4-a716-446655440002', '1dOIqpdKj24Fhm2dG6NLsIbCCzp/u1FtOr93qkrC1uQHIPKE/ma7//kqIs+LllTlVUgqeWg3mRcvjnOCkdjeyw==', '2025-09-19 08:08:41.474685', '2025-09-12 08:08:41.474684');
INSERT INTO auth.sessions VALUES ('fcdf5bd1-b0fe-49ec-887d-d7b184571215', '550e8400-e29b-41d4-a716-446655440002', '0HyJJpiLlkmC/pj5xMC1r2mHUE9keMkYUyA40DL8YU2Vgn/ZAY+a6sBrTjGngl0xdfLtwTpktR/dIB/Hh9Q+vw==', '2025-09-19 08:12:26.569055', '2025-09-12 08:12:26.568987');
INSERT INTO auth.sessions VALUES ('1a537796-fb7c-4533-b2b8-e073c9cda416', '550e8400-e29b-41d4-a716-446655440002', 'z7u3yIzs5zPr96ZneiOgRWbKYMmdwT/9Eze8hxwFisWCOWsOO9JtCa+sXnZsIF8IoOfBiH/WRh9I2hKylZDTfw==', '2025-09-19 10:51:54.232557', '2025-09-12 10:51:54.232309');
INSERT INTO auth.sessions VALUES ('a081419f-afbe-461f-a40d-ccceed230e39', '550e8400-e29b-41d4-a716-446655440002', 'IBcHVAJIc6ADcMZPdh4UYZiJglicZRPnNnQIm6wiBP41S2SIr6OCbDv5TKPxgvZ3M/fyL/2Vtg030BBFvr6Spw==', '2025-09-19 19:14:31.538257', '2025-09-12 19:14:31.53813');
INSERT INTO auth.sessions VALUES ('b23fbc7a-e60c-45e0-aee3-7043c0c84c30', '550e8400-e29b-41d4-a716-446655440002', 'sPyQTkB5CNhbwQxwwAqN+W3CrDbeJFYvZbIDR+pB+ePEG1fS3V/i2yFmYWxXOxJvEDVmDYnOZiNG7oBqs/cLqA==', '2025-09-19 19:32:46.212135', '2025-09-12 19:32:46.212134');
INSERT INTO auth.sessions VALUES ('c3bc3d87-7069-43cf-9e87-20e8ab86cbc0', '550e8400-e29b-41d4-a716-446655440002', 'Ny6+RphUf4a7Jnr74eDqfu5bZN5a60SDIzxEeIZBVYQahijMnfHc3+KlBl8pezGpfhFuFtP3Qh2eI5eJMryMRA==', '2025-09-19 20:02:20.892882', '2025-09-12 20:02:20.892803');
INSERT INTO auth.sessions VALUES ('d863c907-1f3f-4e82-8abc-c24d323d93a5', '550e8400-e29b-41d4-a716-446655440002', 'n2RWUnrDKJUCVM1O7J7Uj9mm0o7Jo9peMgghx0dpPy11o6DjXYLlUni3MLW7gIH2CY8hwgPztoXS9AurpUinHg==', '2025-09-19 20:52:51.283745', '2025-09-12 20:52:51.283745');
INSERT INTO auth.sessions VALUES ('21b000da-8a6c-4f51-86d7-bfd46bce86d8', '550e8400-e29b-41d4-a716-446655440002', 'pHTRT27FfzUMTgZqNSgoBKXgvbe/pgLfM+ar41gtFdDVs7MIm5vmMrDSeRZC9tGy3Wr8jnZGqDaAivwzpgncIg==', '2025-09-19 20:52:59.910118', '2025-09-12 20:52:59.910117');
INSERT INTO auth.sessions VALUES ('8fde91f0-631e-433a-94cb-29587a8ad7e3', '550e8400-e29b-41d4-a716-446655440002', '8UC2U2NoXdOC6kmMzTXGDdy44peaK+wIvJpHObNn1+ENAxtIfZCnrKQ55g92Lb6sqXgO3umrdKtcO+jLDMQLUQ==', '2025-09-19 20:53:16.871009', '2025-09-12 20:53:16.871009');
INSERT INTO auth.sessions VALUES ('742aada2-314f-45b3-8bb8-75d9a90e0a09', '550e8400-e29b-41d4-a716-446655440002', '06vWHsrWtn/zk45QGkjh/r1LwSX8Z2LD2uENp/A52S2vlRr4hAYRYgfh6w0O25dTixKin17C4EohrIEbjFxqJA==', '2025-09-19 20:54:05.33358', '2025-09-12 20:54:05.333579');
INSERT INTO auth.sessions VALUES ('ce9c0a62-0513-46f7-9fcc-f98f5f85d074', '550e8400-e29b-41d4-a716-446655440002', 'eRtMDzQiETLIYdblilSe30AlX/SFFKOSj/vgqpn0LtQt7bpYkhJOAOjzEWy+NaU2/KOxS8QnicGSfQ9OviigiA==', '2025-09-19 20:57:49.534575', '2025-09-12 20:57:49.534574');
INSERT INTO auth.sessions VALUES ('c4781149-9e3f-4991-9409-800f3ad73ad8', '550e8400-e29b-41d4-a716-446655440002', 'feSq+mHFli46zurpJPkBypVhHrvz9Mw3xgqba3J2WYIU2OIZMsmsm6E+AdqNy0NUjZ0jp3HBU78pk1Q35IXJ+g==', '2025-09-19 22:05:43.944344', '2025-09-12 22:05:43.944343');
INSERT INTO auth.sessions VALUES ('64ca3fd0-d58e-4bb8-ae3c-c389733520b7', '550e8400-e29b-41d4-a716-446655440002', 'QgLTsZ+b9bVELcKMpGTR3VfH6wKUZYMbdx5Q6m6ClgPh49yCMjChXa5uto2yq8D8Mu/4n4w/Abyvz0baK7hASg==', '2025-09-19 22:12:13.806878', '2025-09-12 22:12:13.806878');
INSERT INTO auth.sessions VALUES ('0539d932-a113-4c78-bcc8-fd70471f1f58', '550e8400-e29b-41d4-a716-446655440002', 'cDXneTR6GcbNilsY4Hn2iGf6uaKfBsdy+rU24YLG5PnG7+5HzBtqhR8us6ir+tvwroddmpBsSaCgMkM0o9xAyQ==', '2025-09-19 22:32:51.398906', '2025-09-12 22:32:51.398905');
INSERT INTO auth.sessions VALUES ('a454e89f-1ce9-463d-a993-b20c5925b244', '550e8400-e29b-41d4-a716-446655440002', 'MHYsrRfcRtCMEEeKv2llfMD0GYC13baLdCJ0LIkf1BGg34CV2Z/Qk0P9aFiZu3z8P1EXJLUJHd6B+ITLET9jgQ==', '2025-09-19 22:34:25.318555', '2025-09-12 22:34:25.318499');
INSERT INTO auth.sessions VALUES ('2399b0aa-b894-4115-8ca0-d75a1f8532d2', '550e8400-e29b-41d4-a716-446655440002', 'gcm67p/dzup71Ngh5TjdsbPu/W01g6mpcxmAmnxJtUFwWoYz6/vG6d9pTwahHUOnMS4E1pgitwjLkQ+5eNqIjQ==', '2025-09-19 23:14:28.577507', '2025-09-12 23:14:28.577506');
INSERT INTO auth.sessions VALUES ('62afc4f6-1840-478e-bf85-ab14cf23b27d', '550e8400-e29b-41d4-a716-446655440002', 'r3F0x/OU6iQ4tlKiBgPZJopyhQ8y6ZBs99IDbUude1ZTR79ygcP0PeYsPAYh0Ltt609gKbW4R+ahTFs/wq7iFg==', '2025-09-19 23:18:07.444441', '2025-09-12 23:18:07.44444');
INSERT INTO auth.sessions VALUES ('4971cd4f-aa62-4b2a-ac5b-6b2165269b51', '550e8400-e29b-41d4-a716-446655440002', 'dUNrKRC/aXdeCD5qQCxwlQbiUvIMhBaanuMGq+NSjsYO4KUbPzlLFMDvprQWN2ATkfYK/GobONwdhjJW/yrJvA==', '2025-09-19 23:18:10.210476', '2025-09-12 23:18:10.210476');
INSERT INTO auth.sessions VALUES ('df1f2540-4cc4-4a99-b0b0-0aeb1b6dba6c', '550e8400-e29b-41d4-a716-446655440002', 'ynzvwG3vZnvThh/LpTFGeeOa7tCzSKQtKwzNX6Vr87g0AJ8g8uwMABOXpf4lwO29cYY++OLeGi/VDk1+/wlv6A==', '2025-09-19 23:18:13.026773', '2025-09-12 23:18:13.026772');
INSERT INTO auth.sessions VALUES ('5b0b46b0-9381-4017-8a70-778eff926c89', '550e8400-e29b-41d4-a716-446655440002', 'r4YFicWdWw9t89k046waZRvDZIz4wCMY0jhg4CTZIVwgxD+x6aJvWZVadXwlAD+oF747CRIf+E9GyRvuCB5CNw==', '2025-09-19 23:18:16.444526', '2025-09-12 23:18:16.444526');
INSERT INTO auth.sessions VALUES ('5469181a-8b6b-4b62-926c-1a2f950c964d', '550e8400-e29b-41d4-a716-446655440002', 'yr9lU40vLeXEBzBEAxdRYkG/LND/PwFonVZzu5NijKO+eppBv3ovOMuWl63m9XFC7ePPLmimHw4LPmWLShF09g==', '2025-09-19 23:18:27.925058', '2025-09-12 23:18:27.925058');
INSERT INTO auth.sessions VALUES ('426b60bc-462a-423b-82b3-73ad16a94881', '550e8400-e29b-41d4-a716-446655440002', 'rsBq7OYPUZ4/dAtvY84i06+AsM/50eFhaKWMLjyPH6IbIxBIy1ODmf6jsvvJEKwFKR35inaM4tM6Q98iPo3uNA==', '2025-09-19 23:18:30.888784', '2025-09-12 23:18:30.888784');
INSERT INTO auth.sessions VALUES ('e390329f-3963-443d-986c-46b596e0fbf6', '550e8400-e29b-41d4-a716-446655440002', 'Yp4AgILTa8AFazNQ2S5++i9Mc2fEsuVO3QiZ2vy7QqQPIJL6Bk78x6h/8AnrDPTypzvKJKGZowWdGRLnl3FwoA==', '2025-09-19 23:18:34.133007', '2025-09-12 23:18:34.133006');
INSERT INTO auth.sessions VALUES ('426e2779-b81e-4ab0-9ef8-a8fe7145495b', '550e8400-e29b-41d4-a716-446655440002', 'xBm2EmQspZkyNhy2P53X0ABum9OZ/6YiHUINYgIc4K4iFDHWhzvI9fIP5+uqO+4puWIc+3DI/E08W7v6urTY4A==', '2025-09-19 23:18:37.642144', '2025-09-12 23:18:37.642143');
INSERT INTO auth.sessions VALUES ('b4a2bf58-445d-4d9e-a652-416988c6dd57', '550e8400-e29b-41d4-a716-446655440002', 'DySzhgIR2h09N3AWC0L3Hwrk+OVxHCM1TiMAiDWa9R3Sv+aXH8QIL5c+evozINFMH86pz8gv6ZsPSjDMj04GOQ==', '2025-09-19 23:18:41.792661', '2025-09-12 23:18:41.79266');
INSERT INTO auth.sessions VALUES ('3db89153-192e-4413-93da-65ea3aadb794', '550e8400-e29b-41d4-a716-446655440002', 'ML9tvNkrD9ZQSTEMD+SivadCCu/s24ZUfK9jRvx6fwTl5pdDaidnBHr5Lvr1hjMpfSkC5LYy2JkSO+sMFMxEuQ==', '2025-09-19 23:18:45.488179', '2025-09-12 23:18:45.488178');
INSERT INTO auth.sessions VALUES ('99566f38-ca1b-49fb-9e01-fbf449d66562', '550e8400-e29b-41d4-a716-446655440002', 'LQrTvuWoFqfecOUCUBU+O2yPIk3wEEUrExqI/AjZEYQsOzVVoJax+SDqOBLU0bDKxTXzu6m+OfU8imgoEpOAQw==', '2025-09-19 23:18:49.911076', '2025-09-12 23:18:49.911076');
INSERT INTO auth.sessions VALUES ('c7301578-c2e6-4dc5-8239-6b77fcc4628a', '550e8400-e29b-41d4-a716-446655440002', '9WZ2ECy8TE6+1QcLwmnSzK7QP0y6hAkOyb5mE68lvj8WUUk1MOBjiuoojgFi7QVLKxw98/iT7S38IaRuMTKKLQ==', '2025-09-19 23:18:54.419902', '2025-09-12 23:18:54.419901');
INSERT INTO auth.sessions VALUES ('b02b9a7e-185f-4eb5-8e4c-5189f9f1ba18', '550e8400-e29b-41d4-a716-446655440002', 'ysy39sSnj0Z2mhTOtViJ10B8z711MEGHw+3SacdCiQHH92RsFHHeB/HXHZpg30JzDAVgFfrqBNka26zQ+1yGIg==', '2025-09-19 23:18:58.472059', '2025-09-12 23:18:58.472059');
INSERT INTO auth.sessions VALUES ('ed39307c-ed34-4325-b044-74353f432ed8', '550e8400-e29b-41d4-a716-446655440002', '1oS/jxyVi7BCxrWLo2e3xjiEka0DJkvWyu44eaZmIQEQZT3yqTD7lU+u13/wmjom/sTozPwYDGUt+oVmkVuUBg==', '2025-09-19 23:19:02.471174', '2025-09-12 23:19:02.471174');
INSERT INTO auth.sessions VALUES ('00443a15-9299-4901-91a0-b743233b48ee', '550e8400-e29b-41d4-a716-446655440002', 'piArzMN8aCrPL43yzIpi1FMcxnvOn5cr36jTNa8TwXSmWldPKK31TCKBB1oDmLNt7qG28FzA65wg8Co8N3vI0w==', '2025-09-19 23:19:08.101413', '2025-09-12 23:19:08.101413');
INSERT INTO auth.sessions VALUES ('d0df1f02-38e3-4d7e-9600-c0a57a998bfa', '550e8400-e29b-41d4-a716-446655440002', 'wkc0WsnaoBHgJQbm81VupG9Z31FVIdxywkwr8GVasMQ++F/chPDqPQvodCa/GvMCvxROFS3VF5sJOqBYEdI92Q==', '2025-09-19 23:19:32.282486', '2025-09-12 23:19:32.282485');
INSERT INTO auth.sessions VALUES ('3d35c60e-2aa6-466b-93f4-31b73de939f5', '550e8400-e29b-41d4-a716-446655440002', 'OhQkOGwFI/lDkfecmzjPVmfB6EqjuthkbrLF/6HWx+wqxHKycWQwGBauT///XKvR+S91OlHpknnZX5pGuD2Neg==', '2025-09-19 23:19:36.441666', '2025-09-12 23:19:36.441665');
INSERT INTO auth.sessions VALUES ('2165a339-4280-49ce-8c72-13fd716452bd', '550e8400-e29b-41d4-a716-446655440002', '8reg4rN2Aivn1NW4mOVKTte7jRJNt7ksV/xUkl71NfnVzv9EoeU26e3U9ns9iNa9a2Kcxw+B7BsUhG0D1Qlojw==', '2025-09-19 23:19:39.991286', '2025-09-12 23:19:39.991286');
INSERT INTO auth.sessions VALUES ('074484bb-3249-480e-8b40-eef49332f3a6', '550e8400-e29b-41d4-a716-446655440002', 'Cf8qO5YxDgDPM+V3R89h7Zs6TrxoT0pGnauA6plJZ01tSmBqAknm0CrCiSu1ufI5/KeU3gC82LdPOfpsIMZXUg==', '2025-09-19 23:19:43.030724', '2025-09-12 23:19:43.030724');
INSERT INTO auth.sessions VALUES ('598f1210-eb9b-4e1d-b5a8-8b812ef4db5f', '550e8400-e29b-41d4-a716-446655440002', '5fhj26ayLiN4wh5GFuBFk/4KASLlWEbYWYD72lF7j7qw9Dbqv+frgu4i7mfHgUZzJKXtq/TTdArbg9YawC/6qw==', '2025-09-19 23:19:46.483938', '2025-09-12 23:19:46.483938');
INSERT INTO auth.sessions VALUES ('27d9fa9c-dd95-473f-8d32-aad534e9382a', '550e8400-e29b-41d4-a716-446655440002', 'DisyuE/GJcIXkvkXs4+EJ5IwNaYTmZ5A5wuDGTy7CPn1dU0lUnhsDxWKqD7PnS6MDztpTvYbtjG2ROXHGzRkZw==', '2025-09-19 23:38:02.682608', '2025-09-12 23:38:02.682608');
INSERT INTO auth.sessions VALUES ('dda5335f-5730-4b40-a6f9-1d19c97bd327', '550e8400-e29b-41d4-a716-446655440002', 'YvWORAqBV/1yubbtMr4zV+sg8jfTTiadrMdsynqC9+AX9qUEtxxtm8K2o9y+VrX7ce1jjZUimE8hEE2ACHZ7Nw==', '2025-09-19 23:40:50.037573', '2025-09-12 23:40:50.037573');
INSERT INTO auth.sessions VALUES ('08c87be0-65b2-4d86-b581-1d5130cf0dfb', '550e8400-e29b-41d4-a716-446655440002', '99yof8daOz+tvOa5BhUm8ZOExNxhGJmYIYUERy+KbcBNqZdoKDzfSzfnP7HZfH6zraHhQhGGLfjYFCzh6cZoaQ==', '2025-09-19 23:45:21.673612', '2025-09-12 23:45:21.673612');
INSERT INTO auth.sessions VALUES ('39a01240-dae1-4259-8ffd-3028a1f24b58', '550e8400-e29b-41d4-a716-446655440002', 'jT7OQyC0sklzsboezr7PvMkp9jrLKmZRt1bwW7VRfF57oWcT7GQNPtiMuOkwxCzsaEqWUDXHrBvoyrQ6KPwgZw==', '2025-09-19 23:45:26.118256', '2025-09-12 23:45:26.118255');
INSERT INTO auth.sessions VALUES ('9600ac08-0fd5-4f4e-bb78-529a64bbe18d', '550e8400-e29b-41d4-a716-446655440002', 'P83S0R4MD947HiDmV+cNfLN+o1GT+w08HV/Jn/i8brJsVOP8FcmLShLc9GRTQJKaPMwGufvGs9LPvmumQPMTvw==', '2025-09-19 23:45:31.36657', '2025-09-12 23:45:31.36657');
INSERT INTO auth.sessions VALUES ('2934da34-3071-4e4c-8d74-32468a70d730', '550e8400-e29b-41d4-a716-446655440002', 'pIosEKg10zWIR5WEBobyiPUWPa8bnM6EecJJ56IwaAZnzW0/BmKwDh4OxdTDnW67CfPNv7cgu0DJnYIf1kub0g==', '2025-09-19 23:46:47.188888', '2025-09-12 23:46:47.188888');
INSERT INTO auth.sessions VALUES ('18ad2a93-2fe5-492a-be75-9d40eb4e1549', '550e8400-e29b-41d4-a716-446655440002', 'oRaOxRDGrautdkumbQC6F8rPfLWRDMpRyJ9NcBLyW5Qyswfk6FVnG3tf/DjSu9GVNCHl9INfI3KnbZucjIJpGw==', '2025-09-19 23:47:16.542957', '2025-09-12 23:47:16.542957');
INSERT INTO auth.sessions VALUES ('2011b169-01b7-4ad8-93f5-90cc2392194c', '550e8400-e29b-41d4-a716-446655440002', '8PJ6xSNMPNBmG9CrZwM3fIipfQGFvBM5kkb6/HYaGybC8aVwX69jlb5p2d6Ed9amKCdsuSMqk9gVAWlCNAFMuQ==', '2025-09-19 23:47:20.667473', '2025-09-12 23:47:20.667473');
INSERT INTO auth.sessions VALUES ('b704fc67-933e-4c5c-83fb-a88c17c068d1', '550e8400-e29b-41d4-a716-446655440002', '6o3XoUW3xTHJ/eHadvh02jwU9DiTQzyvBxwJDLb2yKtt1311UuSvqKITMoY3XSIPGl1uAxkJjDXflxOfdIRZ3A==', '2025-09-19 23:47:34.809566', '2025-09-12 23:47:34.809566');
INSERT INTO auth.sessions VALUES ('caaf57bc-5f1e-4f63-aed2-8356a2d6ef20', '550e8400-e29b-41d4-a716-446655440002', 'pPjvCmwczoxoQc/J4S05VLRgZAs4eNMJLOInbqLheDCsoCkfg17CATn+WgFfR0zm4TWoypaEcUUCBWTW2c7IzQ==', '2025-09-19 23:47:39.305826', '2025-09-12 23:47:39.305826');
INSERT INTO auth.sessions VALUES ('5a37d9f0-f819-4e8e-9732-06fc0fecf3ae', '550e8400-e29b-41d4-a716-446655440002', 'z2C+W0Swa0k1N9sOuJL65gRlJZ5BPyBl4JFA0INim/jLLHleKeB1gQmvxOq79/eXnMDEZGLgE/4syiHBop/uGA==', '2025-09-19 23:47:47.985761', '2025-09-12 23:47:47.985761');
INSERT INTO auth.sessions VALUES ('202dc220-b36f-4e94-80ef-9a2151b6aba2', '550e8400-e29b-41d4-a716-446655440002', 'UNhUiF4AcU+beL4sZRFpVpfYBiZjkSBW+jFzjryKqvy5lXyYBNSWVUKxddyPSx5xRgLyUjw3bkp7liuMu1LDgQ==', '2025-09-19 23:47:58.736459', '2025-09-12 23:47:58.736459');
INSERT INTO auth.sessions VALUES ('213d3485-fa9c-40b3-a99a-45810f4c3c8a', '550e8400-e29b-41d4-a716-446655440002', 'n4n1YA5SXZ259hJMI2qjoRla+WIul0H4nT/h+tQ6g5rDEcT7bSOTIrB3IGa0zp0tWBdUKhfepk4EOU+YZSqHgw==', '2025-09-19 23:48:50.589937', '2025-09-12 23:48:50.589936');
INSERT INTO auth.sessions VALUES ('78a93756-06cc-421a-9152-f365b90a99de', '550e8400-e29b-41d4-a716-446655440002', 'eJr3oTe8FiT3dQimkh6vrJB7qYF70ziRUnM5yVIsE3v+wVlQ2IhAxQDfGCE4Fb8z5UZypd9IGLMJ+xVqlQB0Eg==', '2025-09-19 23:50:49.498475', '2025-09-12 23:50:49.498475');
INSERT INTO auth.sessions VALUES ('61e6caf2-1871-4670-95fb-55a8400078aa', '550e8400-e29b-41d4-a716-446655440002', 'yGzfJl4kzyx3MYTueMadWdISaUokf/ZaQEk0BLhkWq9Mex2wgAq6blsGjxVLE8YiP5tXznZpU+kcOIP00MWbZQ==', '2025-09-19 23:52:35.290326', '2025-09-12 23:52:35.290326');
INSERT INTO auth.sessions VALUES ('3934ce7b-01d7-45bf-a8a5-aac68677b7f7', '550e8400-e29b-41d4-a716-446655440002', 'hzAT5zYS9ClbfwKwAdMGZjxXDTmCQV/liFZSWI/LyO69dSgNbLzWFlje/iCm1GumPU1XYMLlPfB0m5Bv2LHRrQ==', '2025-09-19 23:52:50.488812', '2025-09-12 23:52:50.488812');
INSERT INTO auth.sessions VALUES ('dc2120b3-a156-4ddb-bab9-3a9f8defe674', '550e8400-e29b-41d4-a716-446655440002', 'ztHSlmkuBtS4KWNTY7cOSkImCjFixtW/gDbjcB8PYM1Ub/VPJAJuaIi91pEyq64HwDSgI94VhvlxacIfotlkLQ==', '2025-09-19 23:53:36.497954', '2025-09-12 23:53:36.497954');
INSERT INTO auth.sessions VALUES ('09351966-9adc-4aaf-a638-77c613a1bf51', '550e8400-e29b-41d4-a716-446655440002', 'fkQ75FT+gyPERvv/Har52Y+JSifyBCBmJ+TA3Eg+HvG1zuvN5XmxMA6ItXCHo8nOrrJWbxKRjcNyAGSv60JDmw==', '2025-09-19 23:56:38.200819', '2025-09-12 23:56:38.200819');
INSERT INTO auth.sessions VALUES ('ef12a493-8fb6-4f8d-877a-da404b244d43', '550e8400-e29b-41d4-a716-446655440002', 'KecCeLc/dCpI8ERMPHTerohqb1D/tmXTrO9BCUo+6QVXiOvYK/B9X0/o7iKcV4YMA5SyUwHjpOuyaH0ro/cX7w==', '2025-09-19 23:58:24.972147', '2025-09-12 23:58:24.972147');
INSERT INTO auth.sessions VALUES ('6ea14af7-c894-4e8d-8c0e-fa43081ef551', '550e8400-e29b-41d4-a716-446655440002', 'N7lxsHDMiRB2LZ88OzqtRtfPRMgS7uI3NvrSPV6grWRmJv/wfu0EG3JLnD82rfhuIet35lkmi3gN1vzfeDHp6A==', '2025-09-19 23:59:43.846562', '2025-09-12 23:59:43.84654');
INSERT INTO auth.sessions VALUES ('7f6a6eee-aeae-4bf5-8808-74d8ed0dcb7f', '550e8400-e29b-41d4-a716-446655440002', '3o335AWIDp8cO4XJQgFugROCK4ov3Ve9t9X5Ft4+0sA8/OxmfbYTIRwXaF7rRGo/YD+4scO+u4qhgF5ZECJfYQ==', '2025-09-19 23:59:48.779384', '2025-09-12 23:59:48.779383');
INSERT INTO auth.sessions VALUES ('9b0a2ef8-c530-4c6b-9a63-38f51ad121c4', '550e8400-e29b-41d4-a716-446655440002', 'gHCULiySkbzn/ueqRh+o5TBRXxL23zHmOXjSVdxVtsJ9XnEhh9USpx+iiwIaAurBFNQabj4ydIDWJwRW0deWEw==', '2025-09-20 00:03:25.486788', '2025-09-13 00:03:25.486727');
INSERT INTO auth.sessions VALUES ('99741898-24b7-4637-9b25-8e45ad8a6883', '550e8400-e29b-41d4-a716-446655440002', 'FkNaCegYxs8w/L4GMRICtqbfNxTGS9kb3Xm8GvhfZ9mQMMGsKxWDzxiABYIdfsdvYr6w5i4foMmZgJxOhYUslg==', '2025-09-20 00:31:15.460484', '2025-09-13 00:31:15.460483');
INSERT INTO auth.sessions VALUES ('30fd52c8-fec0-4bfd-96d6-b9e8ded6f49d', '550e8400-e29b-41d4-a716-446655440002', 'yHX2F0WhMfNFtlRW097AA1dDbnDnsvL9XIF+mJqNd8Xrzv7CgU/O05qh1awDsHzRQQTIQzYL3B0gdG49P1nlGw==', '2025-09-20 01:14:15.494016', '2025-09-13 01:14:15.493933');
INSERT INTO auth.sessions VALUES ('07e48ac1-91f1-4530-a01f-7ca314650da8', '550e8400-e29b-41d4-a716-446655440002', 'RVx+p+yLC1LVrrrbk69oflwO7kLpNTUjYNUsGit356VSgpAlYKaa5Y3OwlqO+/vxnVGAXLYR/9G744UwJWy4Ow==', '2025-09-20 01:20:15.263687', '2025-09-13 01:20:15.263592');
INSERT INTO auth.sessions VALUES ('1b67cb6a-d535-4e27-bb6b-8c3d5d0a4b51', '550e8400-e29b-41d4-a716-446655440002', 'YJI0lf9RjYFsVbDxTP6vpE/s1f3ZxO2DQg314x/pnUg3trFbLH0LNAhamM5e3GyJ6mpvrcGwIqjy9rvpekIZ1w==', '2025-09-20 15:25:21.433711', '2025-09-13 15:25:21.433709');
INSERT INTO auth.sessions VALUES ('f55df657-3389-4364-b180-5045179ed3fb', '550e8400-e29b-41d4-a716-446655440002', 'qEMxmMnLkWHkNM2S4oIvocy75A9qZfh99JS5oAQ4jqcffJ8Td7nn7WI/EYoibp3B7PfusQs8UqN45QKog21cqg==', '2025-09-20 15:56:13.749668', '2025-09-13 15:56:13.749642');
INSERT INTO auth.sessions VALUES ('5958c2a2-d1cd-42a3-8a6a-6cb1b4c8d95f', '550e8400-e29b-41d4-a716-446655440002', 'Ya3EPq+xC+z9yk8bzXYcZKHJeGMgq/NV4cazchS8GLUUe2vzO9ZmwSKsN6OODrDpF+KcoKm4KGh2g8in0pTtYQ==', '2025-09-20 15:56:52.474717', '2025-09-13 15:56:52.474715');
INSERT INTO auth.sessions VALUES ('eec1053d-b1c3-4c43-b727-c3634aa11531', '550e8400-e29b-41d4-a716-446655440002', 'TxA6SLGanR2RFu+RacBfGKDq0V3M0vp0MjEjPUP1+7CfKJNjQvL6xJsxJNLmau9FaL+JkoOwt0RVMo/4sTb0dQ==', '2025-09-20 15:57:12.219756', '2025-09-13 15:57:12.219753');
INSERT INTO auth.sessions VALUES ('d9e40e1a-9417-47ab-a3e9-b20ab4a84322', '550e8400-e29b-41d4-a716-446655440002', 'nWOi/OW3j2LfhJz3rouR1UMGFUeKUZ6Hm5O11+g+WC7bXP7cl2wQfBHxUvtg5MzamwMW1KsdejwdhI9xwJLvOg==', '2025-09-20 15:57:49.153596', '2025-09-13 15:57:49.153594');
INSERT INTO auth.sessions VALUES ('8ec54efc-0018-45ae-bc34-f69863012394', '550e8400-e29b-41d4-a716-446655440002', 'dJNqUGkl00gi6yAJ8TpS00eHX52odPsrdQy/av6HYMEHHufawQ77jBxrY0EoucpN3kPSYuqeEf3FOM7ITsfeHA==', '2025-09-20 16:01:10.036738', '2025-09-13 16:01:10.036737');
INSERT INTO auth.sessions VALUES ('9d873e06-6d1d-4385-b422-1573984c45aa', '550e8400-e29b-41d4-a716-446655440002', '3nq8hO9zLT7s8VYzhesPR1q73P30h4jO664bD3TqARuBMTLp179mh/xz35O+L07a3miYmuCEI+FIP8h600r+bg==', '2025-09-20 16:01:37.847079', '2025-09-13 16:01:37.847077');
INSERT INTO auth.sessions VALUES ('8f0be0aa-d3d5-46ab-8dd9-b2c1290bc86f', '550e8400-e29b-41d4-a716-446655440002', 'QDgXoTspgK/vjTESWutATqGcpBr1jjMSEvqGzgoqsVvIdEWoX/Iizm55ldaKfYywJpHv+jRskDAGdkSAfHvPRw==', '2025-09-20 16:09:35.116596', '2025-09-13 16:09:35.116594');
INSERT INTO auth.sessions VALUES ('399393ca-bc09-40ac-aecb-a22c02b62700', '1ad2e8f0-c0f6-49c7-b03c-46dc0e6c8cd2', 'PhKNfMpxOIo7R0kaV0b2LCFKOnSn0glqXCylKnJvgsph/okSUlpnz3TKsdt0/0gO8gwnyKsFEYzz9aOjYti3fQ==', '2025-09-20 16:18:13.649884', '2025-09-13 16:18:13.649882');
INSERT INTO auth.sessions VALUES ('71ac2a05-075b-4d9c-bc29-910313734a50', '550e8400-e29b-41d4-a716-446655440002', '6zshZAzBWkfioWbfS9eom+T5A+EqNuN5Vty/6WFfjm24lPC/ln9Cy5EAUdJ6kpAfkqcGa9Bb3TwVruI+lBAKIQ==', '2025-09-20 18:20:29.56592', '2025-09-13 18:20:29.565918');
INSERT INTO auth.sessions VALUES ('76094aeb-87a3-401f-a5fc-8f75a008c4fe', '550e8400-e29b-41d4-a716-446655440002', 'bAh1Ldoz/qRTDt2m3uYEeB775aE5UqUn0xSORfgE+5DZPY/2kY2qpwnNLXNXctg6IUK0Cw7IcL0KOVfMXYZz/w==', '2025-09-21 05:45:15.778295', '2025-09-14 05:45:15.778293');
INSERT INTO auth.sessions VALUES ('e5231cb5-31bd-43ca-a740-33c283cd5cb8', '550e8400-e29b-41d4-a716-446655440002', 'iceOaCx3CMSwVC1RYPE/RqeHMPds4NoCVEVoi2VinyX+BcqH8xwji0rB6+/VaUSZ+xJiprNtXT16EMCm1OdC7Q==', '2025-09-21 07:05:19.601307', '2025-09-14 07:05:19.601213');
INSERT INTO auth.sessions VALUES ('369e9599-ea0d-4af2-9a75-93432a10059f', '550e8400-e29b-41d4-a716-446655440002', 'j+Adtyqo239AP6zuM5kfNaMtqUXnVEkKrqI1Eovr4kUhx5sPqH6VgVsvjm/FugGAcLyzLoWIGbz+oonFzhgRIQ==', '2025-09-21 07:14:35.867191', '2025-09-14 07:14:35.867099');
INSERT INTO auth.sessions VALUES ('5eeb25e7-045c-458e-b983-6dd3490dd09a', '550e8400-e29b-41d4-a716-446655440002', 'RmK2MrZ1TwDL02alGtvpucV6gVMw2R2TWKllSKMME7JTa6CplNM5kC0AApY7TmtYqLOFENzgsCGNqh5m4O3dLw==', '2025-09-21 07:18:55.912263', '2025-09-14 07:18:55.912261');
INSERT INTO auth.sessions VALUES ('069aac7b-d8c1-4c28-a2a6-3eb5edd4e656', '550e8400-e29b-41d4-a716-446655440002', 'O/xINUChxgMKmLevURzRV9FtMF/0cKA/XbqdVNMFnCHKOF7M3nsb+ov395whlOY3xBccXZomMw2Utu+ckyNNAA==', '2025-09-21 07:34:21.503063', '2025-09-14 07:34:21.502973');
INSERT INTO auth.sessions VALUES ('3da6ca16-e74d-47bc-9094-40c8f1174a28', '550e8400-e29b-41d4-a716-446655440002', 'zf2HzBK1IcTz1f3N/Xen4DkstDYRu9IPamD6SLn27lcEZ46temGwtuTifJSecVWMi688aKhtU4KupoyZ+uGKkg==', '2025-09-21 07:39:33.382955', '2025-09-14 07:39:33.382953');
INSERT INTO auth.sessions VALUES ('14f39b9a-760d-41f9-b262-e9ab5055082d', '550e8400-e29b-41d4-a716-446655440002', 'Ryt1WYw5UhzLjnnrk5UWDleFTC47khbF1RdVxBDF3gXPrl2E80Avkp4sv5hdzo9yo5MRUMPZeI1jQZIt1KviiQ==', '2025-09-21 07:50:24.512022', '2025-09-14 07:50:24.511932');
INSERT INTO auth.sessions VALUES ('668cafb5-bf0e-4f8d-a116-b213a5e55800', '550e8400-e29b-41d4-a716-446655440002', '2pYFPiite1+qE8/VnL4RBn8wdR0pH4atBlCIA96Ottaqlkrr+B652XrFPK+siz1SWBNqf5CQU5tyw/TSA/T9Zg==', '2025-09-21 07:50:37.300614', '2025-09-14 07:50:37.300611');
INSERT INTO auth.sessions VALUES ('0add2633-50fc-458f-83cc-451c43a85fd5', '550e8400-e29b-41d4-a716-446655440002', 'xb5K+4sOnlU/Eqhd8b8JOZD9aNQdTNohTRwKcDi2wVSjDzps+He6k/UdvXZESnAyx0789ggGv45Ny6St9PVvFw==', '2025-09-21 08:03:05.55045', '2025-09-14 08:03:05.550375');
INSERT INTO auth.sessions VALUES ('e87a7c69-659d-4de2-b0aa-31f37edf03e2', '550e8400-e29b-41d4-a716-446655440002', 'y8SVADnYo9FO0toHW8gfOSAJrqFrW5QpG7+F3+hwm7NFVMfpWn0IudIjcTGFx2PaAmeBTrTdS9xN+J5A4+E5QQ==', '2025-09-21 08:05:25.146689', '2025-09-14 08:05:25.146686');
INSERT INTO auth.sessions VALUES ('0b3f51b9-0214-4745-bb9d-b4e288c8daa5', '550e8400-e29b-41d4-a716-446655440002', 'PvyDeWlswci4MvghfAE92pzIoBVnN77vwfoGPkgct6EyTbIXRjqdtjiKECQ6c0Zq57GHZ11e6WYOCvqjf7XMcQ==', '2025-09-21 08:18:47.736346', '2025-09-14 08:18:47.736343');


--
-- Data for Name: user_sessions; Type: TABLE DATA; Schema: auth; Owner: -
--



--
-- Data for Name: users; Type: TABLE DATA; Schema: auth; Owner: -
--

INSERT INTO auth.users VALUES ('550e8400-e29b-41d4-a716-446655440001', 'admin@planbookai.com', '$2a$06$TR3M6zDkfP0uviJmf4smxOZZl27bTP2xVBkRu46csEypzXjdY5rUe', 1, true, NULL, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764', NULL, false);
INSERT INTO auth.users VALUES ('550e8400-e29b-41d4-a716-446655440002', 'teacher@test.com', '$2a$06$iTsgec8JGNBPUF.Bf7axTOzWhxQ9q8AZ8AIDpAsu5SDZ9KlbR6a4u', 4, true, '2025-09-14 08:18:47.720473', '2025-09-12 07:40:16.04764', '2025-09-14 08:18:47.721816', NULL, false);
INSERT INTO auth.users VALUES ('4ffec90d-3558-47d8-8b4a-7b78da0f08b4', 'giao_vien@example.com', '$2a$11$xWqN.CSvVeeb2CHiUgxd.e3VHsZwxO.3GupRY5yZaSWXF0gTTVS6.', 4, true, NULL, '2025-09-12 23:17:56.000754', '2025-09-12 23:17:56.000754', NULL, false);
INSERT INTO auth.users VALUES ('741a7cf5-d7fe-4393-9eda-011c01f6cc9e', 'nguyena@gmail.com', '$2a$11$D.4ZtPKKXMVCmQSou.KOvOj0k3iKOE79oV.4vCFNHh4wHIJpLttjG', 4, true, '2025-09-13 14:48:46.467721', '2025-09-13 14:48:35.996258', '2025-09-13 14:48:46.469219', NULL, false);
INSERT INTO auth.users VALUES ('1ad2e8f0-c0f6-49c7-b03c-46dc0e6c8cd2', '2251120302@ut.edu.vn', '$2a$11$Ary9.i74CXm9q9fXUIYR.eiO6P3I5tljl5pwREb5FZR.NvX.D74fy', 4, true, '2025-09-13 16:42:11.78223', '2025-09-13 16:17:55.549768', '2025-09-13 16:42:11.783008', NULL, false);


--
-- Data for Name: chu_de; Type: TABLE DATA; Schema: content; Owner: -
--

INSERT INTO content.chu_de VALUES ('bece3563-0ee0-4623-96d3-57e1af7ea39f', 'Cấu trúc nguyên tử', 'Nghiên cứu về cấu trúc và tính chất của nguyên tử', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO content.chu_de VALUES ('f109f1be-3eb1-42bc-a1a3-adbf723b1875', 'Bảng tuần hoàn', 'Nghiên cứu về bảng tuần hoàn các nguyên tố hóa học', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO content.chu_de VALUES ('4ff055b6-f594-4225-881f-8111ca09af53', 'Liên kết hóa học', 'Nghiên cứu về các loại liên kết hóa học', 'HOA_HOC', 10, NULL, '550e8400-e29b-41d4-a716-446655440002', '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');


--
-- Data for Name: lesson_plans; Type: TABLE DATA; Schema: content; Owner: -
--



--
-- Data for Name: lesson_templates; Type: TABLE DATA; Schema: content; Owner: -
--

INSERT INTO content.lesson_templates VALUES ('efd6d407-e795-4c08-ad12-016d8b32c5e8', 'Chương 1: Các Loại AXIT', 'Tìm hiểu về các loại axit', '{"CauTrucChung": {"MucTieu": ["Kiến thức", "Kỹ năng", "Thái độ"], "NoiDungKhung": ["Ổn định", "Bài mới", "Củng cố", "Dặn dò"]}}', 'HOA_HOC', 10, '550e8400-e29b-41d4-a716-446655440002', 'INACTIVE', '2025-09-13 17:57:05.336913', '2025-09-13 17:57:05.336955');


--
-- Data for Name: file_metadata; Type: TABLE DATA; Schema: files; Owner: -
--



--
-- Data for Name: file_storage; Type: TABLE DATA; Schema: files; Owner: -
--



--
-- Data for Name: performance_metrics; Type: TABLE DATA; Schema: logging; Owner: -
--



--
-- Data for Name: system_logs; Type: TABLE DATA; Schema: logging; Owner: -
--



--
-- Data for Name: email_queue; Type: TABLE DATA; Schema: notifications; Owner: -
--



--
-- Data for Name: notifications; Type: TABLE DATA; Schema: notifications; Owner: -
--



--
-- Data for Name: answer_sheets; Type: TABLE DATA; Schema: students; Owner: -
--



--
-- Data for Name: classes; Type: TABLE DATA; Schema: students; Owner: -
--



--
-- Data for Name: student_results; Type: TABLE DATA; Schema: students; Owner: -
--



--
-- Data for Name: students; Type: TABLE DATA; Schema: students; Owner: -
--



--
-- Data for Name: activity_logs; Type: TABLE DATA; Schema: users; Owner: -
--



--
-- Data for Name: otp_codes; Type: TABLE DATA; Schema: users; Owner: -
--



--
-- Data for Name: password_history; Type: TABLE DATA; Schema: users; Owner: -
--



--
-- Data for Name: user_profiles; Type: TABLE DATA; Schema: users; Owner: -
--

INSERT INTO users.user_profiles VALUES ('87209c76-3e2a-4538-b3ed-48ac22a18913', '550e8400-e29b-41d4-a716-446655440001', 'Admin System', '0123456789', 'Hà Nội', 'Quản trị viên hệ thống', NULL, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');
INSERT INTO users.user_profiles VALUES ('8889c7bc-2751-47ed-ac61-24ba3cf3c0d5', '550e8400-e29b-41d4-a716-446655440002', 'Giáo viên Test', '0987654321', 'TP.HCM', 'Giáo viên Hóa học THPT', NULL, '2025-09-12 07:40:16.04764', '2025-09-12 07:40:16.04764');


--
-- Data for Name: user_sessions; Type: TABLE DATA; Schema: users; Owner: -
--



--
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: auth; Owner: -
--

SELECT pg_catalog.setval('auth.roles_id_seq', 1, false);


--
-- Name: exam_questions exam_questions_exam_id_question_id_key; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_question_id_key UNIQUE (exam_id, question_id);


--
-- Name: exam_questions exam_questions_exam_id_question_order_key; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_question_order_key UNIQUE (exam_id, question_order);


--
-- Name: exam_questions exam_questions_pkey; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_pkey PRIMARY KEY (id);


--
-- Name: exam_templates exam_templates_pkey; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_templates
    ADD CONSTRAINT exam_templates_pkey PRIMARY KEY (id);


--
-- Name: exams exams_pkey; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_pkey PRIMARY KEY (id);


--
-- Name: question_choices question_choices_pkey; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_pkey PRIMARY KEY (id);


--
-- Name: question_choices question_choices_question_id_choice_order_key; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_question_id_choice_order_key UNIQUE (question_id, choice_order);


--
-- Name: questions questions_pkey; Type: CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.questions
    ADD CONSTRAINT questions_pkey PRIMARY KEY (id);


--
-- Name: email_verifications email_verifications_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_pkey PRIMARY KEY (id);


--
-- Name: email_verifications email_verifications_verification_token_key; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_verification_token_key UNIQUE (verification_token);


--
-- Name: password_history password_history_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.password_history
    ADD CONSTRAINT password_history_pkey PRIMARY KEY (id);


--
-- Name: password_resets password_resets_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.password_resets
    ADD CONSTRAINT password_resets_pkey PRIMARY KEY (id);


--
-- Name: roles roles_name_key; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.roles
    ADD CONSTRAINT roles_name_key UNIQUE (name);


--
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- Name: sessions sessions_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_pkey PRIMARY KEY (id);


--
-- Name: sessions sessions_token_key; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_token_key UNIQUE (token);


--
-- Name: user_sessions user_sessions_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_pkey PRIMARY KEY (id);


--
-- Name: user_sessions user_sessions_session_token_key; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_session_token_key UNIQUE (session_token);


--
-- Name: users users_email_key; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: chu_de chu_de_pkey; Type: CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_pkey PRIMARY KEY (id);


--
-- Name: lesson_plans lesson_plans_pkey; Type: CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_pkey PRIMARY KEY (id);


--
-- Name: lesson_templates lesson_templates_pkey; Type: CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_templates
    ADD CONSTRAINT lesson_templates_pkey PRIMARY KEY (id);


--
-- Name: file_metadata file_metadata_file_id_key_key; Type: CONSTRAINT; Schema: files; Owner: -
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_file_id_key_key UNIQUE (file_id, key);


--
-- Name: file_metadata file_metadata_pkey; Type: CONSTRAINT; Schema: files; Owner: -
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_pkey PRIMARY KEY (id);


--
-- Name: file_storage file_storage_pkey; Type: CONSTRAINT; Schema: files; Owner: -
--

ALTER TABLE ONLY files.file_storage
    ADD CONSTRAINT file_storage_pkey PRIMARY KEY (id);


--
-- Name: performance_metrics performance_metrics_pkey; Type: CONSTRAINT; Schema: logging; Owner: -
--

ALTER TABLE ONLY logging.performance_metrics
    ADD CONSTRAINT performance_metrics_pkey PRIMARY KEY (id);


--
-- Name: system_logs system_logs_pkey; Type: CONSTRAINT; Schema: logging; Owner: -
--

ALTER TABLE ONLY logging.system_logs
    ADD CONSTRAINT system_logs_pkey PRIMARY KEY (id);


--
-- Name: email_queue email_queue_pkey; Type: CONSTRAINT; Schema: notifications; Owner: -
--

ALTER TABLE ONLY notifications.email_queue
    ADD CONSTRAINT email_queue_pkey PRIMARY KEY (id);


--
-- Name: notifications notifications_pkey; Type: CONSTRAINT; Schema: notifications; Owner: -
--

ALTER TABLE ONLY notifications.notifications
    ADD CONSTRAINT notifications_pkey PRIMARY KEY (id);


--
-- Name: answer_sheets answer_sheets_pkey; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_pkey PRIMARY KEY (id);


--
-- Name: classes classes_pkey; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.classes
    ADD CONSTRAINT classes_pkey PRIMARY KEY (id);


--
-- Name: student_results student_results_pkey; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_pkey PRIMARY KEY (id);


--
-- Name: student_results student_results_student_id_exam_id_key; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_student_id_exam_id_key UNIQUE (student_id, exam_id);


--
-- Name: students students_owner_teacher_id_student_code_key; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_owner_teacher_id_student_code_key UNIQUE (owner_teacher_id, student_code);


--
-- Name: students students_pkey; Type: CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_pkey PRIMARY KEY (id);


--
-- Name: activity_logs activity_logs_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.activity_logs
    ADD CONSTRAINT activity_logs_pkey PRIMARY KEY (id);


--
-- Name: otp_codes otp_codes_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.otp_codes
    ADD CONSTRAINT otp_codes_pkey PRIMARY KEY (id);


--
-- Name: password_history password_history_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_pkey PRIMARY KEY (id);


--
-- Name: user_profiles user_profiles_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.user_profiles
    ADD CONSTRAINT user_profiles_pkey PRIMARY KEY (id);


--
-- Name: user_sessions user_sessions_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_pkey PRIMARY KEY (id);


--
-- Name: user_sessions user_sessions_session_token_key; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_session_token_key UNIQUE (session_token);


--
-- Name: idx_assessment_exam_templates_created_by; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_exam_templates_created_by ON assessment.exam_templates USING btree (created_by);


--
-- Name: idx_assessment_exam_templates_grade; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_exam_templates_grade ON assessment.exam_templates USING btree (grade);


--
-- Name: idx_assessment_exam_templates_status; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_exam_templates_status ON assessment.exam_templates USING btree (status);


--
-- Name: idx_assessment_exam_templates_subject; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_exam_templates_subject ON assessment.exam_templates USING btree (subject);


--
-- Name: idx_assessment_exams_subject; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_exams_subject ON assessment.exams USING btree (subject);


--
-- Name: idx_assessment_exams_teacher; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_exams_teacher ON assessment.exams USING btree (teacher_id);


--
-- Name: idx_assessment_exams_template; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_exams_template ON assessment.exams USING btree (template_id);


--
-- Name: idx_assessment_question_choices_question; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_question_choices_question ON assessment.question_choices USING btree (question_id);


--
-- Name: idx_assessment_questions_created_by; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_questions_created_by ON assessment.questions USING btree (created_by);


--
-- Name: idx_assessment_questions_difficulty; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_questions_difficulty ON assessment.questions USING btree (difficulty);


--
-- Name: idx_assessment_questions_subject; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_questions_subject ON assessment.questions USING btree (subject);


--
-- Name: idx_assessment_questions_topic; Type: INDEX; Schema: assessment; Owner: -
--

CREATE INDEX idx_assessment_questions_topic ON assessment.questions USING btree (topic);


--
-- Name: idx_content_chu_de_parent; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_content_chu_de_parent ON content.chu_de USING btree (parent_id);


--
-- Name: idx_content_chu_de_subject; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_content_chu_de_subject ON content.chu_de USING btree (subject);


--
-- Name: idx_content_lesson_plans_subject; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_content_lesson_plans_subject ON content.lesson_plans USING btree (subject);


--
-- Name: idx_content_lesson_plans_teacher; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_content_lesson_plans_teacher ON content.lesson_plans USING btree (teacher_id);


--
-- Name: idx_content_lesson_templates_subject; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_content_lesson_templates_subject ON content.lesson_templates USING btree (subject);


--
-- Name: idx_lesson_plans_topic; Type: INDEX; Schema: content; Owner: -
--

CREATE INDEX idx_lesson_plans_topic ON content.lesson_plans USING btree (topic_id);


--
-- Name: idx_files_metadata_file; Type: INDEX; Schema: files; Owner: -
--

CREATE INDEX idx_files_metadata_file ON files.file_metadata USING btree (file_id);


--
-- Name: idx_files_storage_file_type; Type: INDEX; Schema: files; Owner: -
--

CREATE INDEX idx_files_storage_file_type ON files.file_storage USING btree (file_type);


--
-- Name: idx_files_storage_status; Type: INDEX; Schema: files; Owner: -
--

CREATE INDEX idx_files_storage_status ON files.file_storage USING btree (status);


--
-- Name: idx_files_storage_uploaded_by; Type: INDEX; Schema: files; Owner: -
--

CREATE INDEX idx_files_storage_uploaded_by ON files.file_storage USING btree (uploaded_by);


--
-- Name: idx_logging_performance_created_at; Type: INDEX; Schema: logging; Owner: -
--

CREATE INDEX idx_logging_performance_created_at ON logging.performance_metrics USING btree (created_at);


--
-- Name: idx_logging_performance_service; Type: INDEX; Schema: logging; Owner: -
--

CREATE INDEX idx_logging_performance_service ON logging.performance_metrics USING btree (service_name);


--
-- Name: idx_logging_system_logs_created_at; Type: INDEX; Schema: logging; Owner: -
--

CREATE INDEX idx_logging_system_logs_created_at ON logging.system_logs USING btree (created_at);


--
-- Name: idx_logging_system_logs_level; Type: INDEX; Schema: logging; Owner: -
--

CREATE INDEX idx_logging_system_logs_level ON logging.system_logs USING btree (level);


--
-- Name: idx_email_queue_status; Type: INDEX; Schema: notifications; Owner: -
--

CREATE INDEX idx_email_queue_status ON notifications.email_queue USING btree (status);


--
-- Name: idx_notifications_created_at; Type: INDEX; Schema: notifications; Owner: -
--

CREATE INDEX idx_notifications_created_at ON notifications.notifications USING btree (created_at);


--
-- Name: idx_notifications_is_read; Type: INDEX; Schema: notifications; Owner: -
--

CREATE INDEX idx_notifications_is_read ON notifications.notifications USING btree (is_read);


--
-- Name: idx_notifications_user; Type: INDEX; Schema: notifications; Owner: -
--

CREATE INDEX idx_notifications_user ON notifications.notifications USING btree (user_id);


--
-- Name: idx_students_answer_sheets_exam; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_answer_sheets_exam ON students.answer_sheets USING btree (exam_id);


--
-- Name: idx_students_answer_sheets_student; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_answer_sheets_student ON students.answer_sheets USING btree (student_id);


--
-- Name: idx_students_classes_teacher; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_classes_teacher ON students.classes USING btree (homeroom_teacher_id);


--
-- Name: idx_students_student_results_exam; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_student_results_exam ON students.student_results USING btree (exam_id);


--
-- Name: idx_students_student_results_student; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_student_results_student ON students.student_results USING btree (student_id);


--
-- Name: idx_students_students_class; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_students_class ON students.students USING btree (class_id);


--
-- Name: idx_students_students_code; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_students_code ON students.students USING btree (student_code);


--
-- Name: idx_students_students_teacher; Type: INDEX; Schema: students; Owner: -
--

CREATE INDEX idx_students_students_teacher ON students.students USING btree (owner_teacher_id);


--
-- Name: idx_users_activity_logs_created; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_activity_logs_created ON users.activity_logs USING btree (created_at);


--
-- Name: idx_users_activity_logs_user; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_activity_logs_user ON users.activity_logs USING btree (user_id);


--
-- Name: idx_users_otp_codes_expires; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_otp_codes_expires ON users.otp_codes USING btree (expires_at);


--
-- Name: idx_users_otp_codes_purpose; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_otp_codes_purpose ON users.otp_codes USING btree (purpose);


--
-- Name: idx_users_otp_codes_user; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_otp_codes_user ON users.otp_codes USING btree (user_id);


--
-- Name: idx_users_password_history_user; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_password_history_user ON users.password_history USING btree (user_id);


--
-- Name: idx_users_profiles_user; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_profiles_user ON users.user_profiles USING btree (user_id);


--
-- Name: idx_users_user_sessions_active; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_user_sessions_active ON users.user_sessions USING btree (is_active);


--
-- Name: idx_users_user_sessions_token; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_user_sessions_token ON users.user_sessions USING btree (session_token);


--
-- Name: idx_users_user_sessions_user; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX idx_users_user_sessions_user ON users.user_sessions USING btree (user_id);


--
-- Name: exam_templates trigger_updated_at_assessment_exam_templates; Type: TRIGGER; Schema: assessment; Owner: -
--

CREATE TRIGGER trigger_updated_at_assessment_exam_templates BEFORE UPDATE ON assessment.exam_templates FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: exams trigger_updated_at_assessment_exams; Type: TRIGGER; Schema: assessment; Owner: -
--

CREATE TRIGGER trigger_updated_at_assessment_exams BEFORE UPDATE ON assessment.exams FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: questions trigger_updated_at_assessment_questions; Type: TRIGGER; Schema: assessment; Owner: -
--

CREATE TRIGGER trigger_updated_at_assessment_questions BEFORE UPDATE ON assessment.questions FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: roles trigger_updated_at_auth_roles; Type: TRIGGER; Schema: auth; Owner: -
--

CREATE TRIGGER trigger_updated_at_auth_roles BEFORE UPDATE ON auth.roles FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: users trigger_updated_at_auth_users; Type: TRIGGER; Schema: auth; Owner: -
--

CREATE TRIGGER trigger_updated_at_auth_users BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: users trigger_updated_at_auth_users_soft_delete; Type: TRIGGER; Schema: auth; Owner: -
--

CREATE TRIGGER trigger_updated_at_auth_users_soft_delete BEFORE UPDATE ON auth.users FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: chu_de trigger_updated_at_content_chu_de; Type: TRIGGER; Schema: content; Owner: -
--

CREATE TRIGGER trigger_updated_at_content_chu_de BEFORE UPDATE ON content.chu_de FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: lesson_plans trigger_updated_at_content_lesson_plans; Type: TRIGGER; Schema: content; Owner: -
--

CREATE TRIGGER trigger_updated_at_content_lesson_plans BEFORE UPDATE ON content.lesson_plans FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: lesson_templates trigger_updated_at_content_lesson_templates; Type: TRIGGER; Schema: content; Owner: -
--

CREATE TRIGGER trigger_updated_at_content_lesson_templates BEFORE UPDATE ON content.lesson_templates FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: file_storage trigger_updated_at_files_file_storage; Type: TRIGGER; Schema: files; Owner: -
--

CREATE TRIGGER trigger_updated_at_files_file_storage BEFORE UPDATE ON files.file_storage FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: classes trigger_updated_at_students_classes; Type: TRIGGER; Schema: students; Owner: -
--

CREATE TRIGGER trigger_updated_at_students_classes BEFORE UPDATE ON students.classes FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: students trigger_updated_at_students_students; Type: TRIGGER; Schema: students; Owner: -
--

CREATE TRIGGER trigger_updated_at_students_students BEFORE UPDATE ON students.students FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: user_profiles trigger_updated_at_users_user_profiles; Type: TRIGGER; Schema: users; Owner: -
--

CREATE TRIGGER trigger_updated_at_users_user_profiles BEFORE UPDATE ON users.user_profiles FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();


--
-- Name: exam_questions exam_questions_exam_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id) ON DELETE CASCADE;


--
-- Name: exam_questions exam_questions_question_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_questions
    ADD CONSTRAINT exam_questions_question_id_fkey FOREIGN KEY (question_id) REFERENCES assessment.questions(id);


--
-- Name: exam_templates exam_templates_created_by_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exam_templates
    ADD CONSTRAINT exam_templates_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- Name: exams exams_teacher_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- Name: exams exams_template_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_template_id_fkey FOREIGN KEY (template_id) REFERENCES assessment.exam_templates(id);


--
-- Name: question_choices question_choices_question_id_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.question_choices
    ADD CONSTRAINT question_choices_question_id_fkey FOREIGN KEY (question_id) REFERENCES assessment.questions(id) ON DELETE CASCADE;


--
-- Name: questions questions_created_by_fkey; Type: FK CONSTRAINT; Schema: assessment; Owner: -
--

ALTER TABLE ONLY assessment.questions
    ADD CONSTRAINT questions_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- Name: email_verifications email_verifications_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.email_verifications
    ADD CONSTRAINT email_verifications_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- Name: password_history password_history_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.password_history
    ADD CONSTRAINT password_history_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- Name: password_resets password_resets_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.password_resets
    ADD CONSTRAINT password_resets_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- Name: sessions sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.sessions
    ADD CONSTRAINT sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- Name: user_sessions user_sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.user_sessions
    ADD CONSTRAINT user_sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- Name: users users_role_id_fkey; Type: FK CONSTRAINT; Schema: auth; Owner: -
--

ALTER TABLE ONLY auth.users
    ADD CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES auth.roles(id);


--
-- Name: chu_de chu_de_created_by_fkey; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- Name: chu_de chu_de_parent_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.chu_de
    ADD CONSTRAINT chu_de_parent_id_fkey FOREIGN KEY (parent_id) REFERENCES content.chu_de(id);


--
-- Name: lesson_plans fk_lesson_plans_topic; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT fk_lesson_plans_topic FOREIGN KEY (topic_id) REFERENCES content.chu_de(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: lesson_plans lesson_plans_teacher_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_teacher_id_fkey FOREIGN KEY (teacher_id) REFERENCES auth.users(id);


--
-- Name: lesson_plans lesson_plans_template_id_fkey; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_plans
    ADD CONSTRAINT lesson_plans_template_id_fkey FOREIGN KEY (template_id) REFERENCES content.lesson_templates(id);


--
-- Name: lesson_templates lesson_templates_created_by_fkey; Type: FK CONSTRAINT; Schema: content; Owner: -
--

ALTER TABLE ONLY content.lesson_templates
    ADD CONSTRAINT lesson_templates_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);


--
-- Name: file_metadata file_metadata_file_id_fkey; Type: FK CONSTRAINT; Schema: files; Owner: -
--

ALTER TABLE ONLY files.file_metadata
    ADD CONSTRAINT file_metadata_file_id_fkey FOREIGN KEY (file_id) REFERENCES files.file_storage(id) ON DELETE CASCADE;


--
-- Name: file_storage file_storage_uploaded_by_fkey; Type: FK CONSTRAINT; Schema: files; Owner: -
--

ALTER TABLE ONLY files.file_storage
    ADD CONSTRAINT file_storage_uploaded_by_fkey FOREIGN KEY (uploaded_by) REFERENCES auth.users(id);


--
-- Name: performance_metrics performance_metrics_user_id_fkey; Type: FK CONSTRAINT; Schema: logging; Owner: -
--

ALTER TABLE ONLY logging.performance_metrics
    ADD CONSTRAINT performance_metrics_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- Name: system_logs system_logs_user_id_fkey; Type: FK CONSTRAINT; Schema: logging; Owner: -
--

ALTER TABLE ONLY logging.system_logs
    ADD CONSTRAINT system_logs_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- Name: notifications notifications_user_id_fkey; Type: FK CONSTRAINT; Schema: notifications; Owner: -
--

ALTER TABLE ONLY notifications.notifications
    ADD CONSTRAINT notifications_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id);


--
-- Name: answer_sheets answer_sheets_exam_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id);


--
-- Name: answer_sheets answer_sheets_student_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.answer_sheets
    ADD CONSTRAINT answer_sheets_student_id_fkey FOREIGN KEY (student_id) REFERENCES students.students(id);


--
-- Name: classes classes_homeroom_teacher_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.classes
    ADD CONSTRAINT classes_homeroom_teacher_id_fkey FOREIGN KEY (homeroom_teacher_id) REFERENCES auth.users(id);


--
-- Name: student_results student_results_exam_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_exam_id_fkey FOREIGN KEY (exam_id) REFERENCES assessment.exams(id);


--
-- Name: student_results student_results_student_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.student_results
    ADD CONSTRAINT student_results_student_id_fkey FOREIGN KEY (student_id) REFERENCES students.students(id);


--
-- Name: students students_class_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_class_id_fkey FOREIGN KEY (class_id) REFERENCES students.classes(id);


--
-- Name: students students_owner_teacher_id_fkey; Type: FK CONSTRAINT; Schema: students; Owner: -
--

ALTER TABLE ONLY students.students
    ADD CONSTRAINT students_owner_teacher_id_fkey FOREIGN KEY (owner_teacher_id) REFERENCES auth.users(id);


--
-- Name: activity_logs activity_logs_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.activity_logs
    ADD CONSTRAINT activity_logs_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- Name: otp_codes otp_codes_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.otp_codes
    ADD CONSTRAINT otp_codes_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- Name: password_history password_history_changed_by_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_changed_by_fkey FOREIGN KEY (changed_by) REFERENCES auth.users(id);


--
-- Name: password_history password_history_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.password_history
    ADD CONSTRAINT password_history_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- Name: user_profiles user_profiles_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.user_profiles
    ADD CONSTRAINT user_profiles_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- Name: user_sessions user_sessions_user_id_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.user_sessions
    ADD CONSTRAINT user_sessions_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict NAmfK93R50azqc8k9PPgWFLJQ4TcK8JZz0VZSwwprwzDeSHqE7cgLycLfZQbkRR

