import { useEffect, useState } from "react";
import "./App.css";

const API = "https://localhost:7032/api";

function App() {
  const [page, setPage] = useState("login");
  const [user, setUser] = useState(
    JSON.parse(localStorage.getItem("user")) || null
  );
  const [token, setToken] = useState(localStorage.getItem("token") || "");

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [name, setName] = useState("");

  const [courses, setCourses] = useState([]);
  const [myCourses, setMyCourses] = useState([]);

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [instructor, setInstructor] = useState("");
  const [editingCourse, setEditingCourse] = useState(null);

  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);

  // LOGIN
  async function login(e) {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    try {
      const res = await fetch(`${API}/Auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });

      if (!res.ok) throw new Error("Invalid email or password");

      const data = await res.json();

      localStorage.setItem("token", data.token);
      localStorage.setItem("user", JSON.stringify(data));

      setToken(data.token);
      setUser(data);
      setPage(data.role === "Admin" ? "admin" : "student");
      setMessage("Login successful!");
    } catch (error) {
      setMessage(error.message);
    }

    setLoading(false);
  }

  // REGISTER
  async function register(e) {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    try {
      const res = await fetch(`${API}/Users/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, email, password }),
      });

      if (!res.ok) throw new Error("Registration failed");

      setMessage("Registration successful. Please login.");
      setPage("login");
      setName("");
      setEmail("");
      setPassword("");
    } catch (error) {
      setMessage(error.message);
    }

    setLoading(false);
  }

  // GET ALL COURSES
  async function loadCourses() {
    try {
      const res = await fetch(`${API}/Courses`);
      if (!res.ok) throw new Error("Failed to load courses");
      setCourses(await res.json());
    } catch (error) {
      setMessage(error.message);
    }
  }

  // GET MY COURSES
  async function loadMyCourses() {
    try {
      const res = await fetch(`${API}/Enrollments/my-courses`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!res.ok) throw new Error("Failed to load your courses");

      setMyCourses(await res.json());
    } catch (error) {
      setMessage(error.message);
    }
  }

  // ENROLL
  async function enroll(courseId) {
    try {
      const res = await fetch(`${API}/Enrollments`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ userId: 0, courseId }),
      });

      if (!res.ok) {
        throw new Error(
          "Enrollment failed. You may already be enrolled."
        );
      }

      setMessage("Successfully enrolled!");
      loadMyCourses();
    } catch (error) {
      setMessage(error.message);
    }
  }

  // CANCEL ENROLLMENT
  async function cancelEnrollment(id) {
    try {
      const res = await fetch(`${API}/Enrollments/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!res.ok) throw new Error("Could not cancel enrollment");

      setMessage("Enrollment cancelled.");
      loadMyCourses();
    } catch (error) {
      setMessage(error.message);
    }
  }

  // ADD COURSE
  async function addCourse(e) {
    e.preventDefault();

    try {
      const res = await fetch(`${API}/Courses`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ title, description, instructor }),
      });

      if (!res.ok) throw new Error("Could not add course");

      setMessage("Course added successfully.");
      clearForm();
      loadCourses();
    } catch (error) {
      setMessage(error.message);
    }
  }

  // START EDIT
  function startEdit(course) {
    setEditingCourse(course);
    setTitle(course.title);
    setDescription(course.description);
    setInstructor(course.instructor);
    setMessage("");
  }

  // UPDATE COURSE
  async function updateCourse(e) {
    e.preventDefault();

    try {
      const res = await fetch(`${API}/Courses/${editingCourse.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ title, description, instructor }),
      });

      if (!res.ok) throw new Error("Could not update course");

      setMessage("Course updated successfully.");
      clearForm();
      loadCourses();
    } catch (error) {
      setMessage(error.message);
    }
  }

  // DELETE COURSE
  async function deleteCourse(id) {
    try {
      const res = await fetch(`${API}/Courses/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!res.ok) throw new Error("Could not delete course");

      setMessage("Course deleted.");
      loadCourses();
    } catch (error) {
      setMessage(error.message);
    }
  }

  // CLEAR COURSE FORM
  function clearForm() {
    setEditingCourse(null);
    setTitle("");
    setDescription("");
    setInstructor("");
  }

  // LOGOUT
  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");

    setToken("");
    setUser(null);
    setPage("login");
    setCourses([]);
    setMyCourses([]);
    clearForm();
  }

  // LOAD DATA WHEN DASHBOARD OPENS
  useEffect(() => {
    if (page === "student" || page === "admin") loadCourses();
    if (page === "student" && token) loadMyCourses();
  }, [page, token]);

  // LOGIN / REGISTER
  if (!user) {
    return (
      <div className="auth-page">
        <div className="auth-card">
          <h1>
            {page === "login"
              ? "Course Management System"
              : "Create Account"}
          </h1>

          <p className="subtitle">
            {page === "login"
              ? "Login to your account"
              : "Register as a student"}
          </p>

          <form onSubmit={page === "login" ? login : register}>
            {page === "register" && (
              <input
                type="text"
                placeholder="Name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            )}

            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />

            <input
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />

            <button type="submit">
              {loading
                ? page === "login"
                  ? "Logging in..."
                  : "Registering..."
                : page === "login"
                ? "Login"
                : "Register"}
            </button>
          </form>

          <p>{message}</p>

          <button
            className="secondary"
            onClick={() => {
              setMessage("");
              setPage(page === "login" ? "register" : "login");
            }}
          >
            {page === "login"
              ? "Create Account"
              : "Back to Login"}
          </button>
        </div>
      </div>
    );
  }

  // STUDENT DASHBOARD
  if (user.role === "Student") {
    return (
      <div className="dashboard">
        <Nav user={user} logout={logout} />

        <main>
          <h1>Student Dashboard</h1>
          <p className="message">{message}</p>

          <section>
            <h2>Available Courses</h2>

            <div className="course-grid">
              {courses.map((course) => (
                <div className="course-card" key={course.id}>
                  <h3>{course.title}</h3>
                  <p>{course.description}</p>
                  <p>
                    <strong>Instructor:</strong>{" "}
                    {course.instructor}
                  </p>

                  <button onClick={() => enroll(course.id)}>
                    Enroll
                  </button>
                </div>
              ))}
            </div>
          </section>

          <section>
            <h2>My Courses</h2>

            <div className="course-grid">
              {myCourses.length === 0 ? (
                <p>No enrolled courses.</p>
              ) : (
                myCourses.map((course) => (
                  <div
                    className="course-card"
                    key={course.id}
                  >
                    <h3>Course ID: {course.courseId}</h3>

                    <button
                      className="danger"
                      onClick={() =>
                        cancelEnrollment(course.id)
                      }
                    >
                      Cancel Enrollment
                    </button>
                  </div>
                ))
              )}
            </div>
          </section>
        </main>
      </div>
    );
  }

  // ADMIN DASHBOARD
  return (
    <div className="dashboard">
      <Nav user={user} logout={logout} />

      <main>
        <h1>Admin Dashboard</h1>
        <p className="message">{message}</p>

        <section className="form-section">
          <h2>
            {editingCourse ? "Edit Course" : "Add Course"}
          </h2>

          <form
            onSubmit={editingCourse ? updateCourse : addCourse}
          >
            <input
              type="text"
              placeholder="Course Title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
            />

            <input
              type="text"
              placeholder="Description"
              value={description}
              onChange={(e) =>
                setDescription(e.target.value)
              }
              required
            />

            <input
              type="text"
              placeholder="Instructor"
              value={instructor}
              onChange={(e) =>
                setInstructor(e.target.value)
              }
              required
            />

            <button type="submit">
              {editingCourse
                ? "Update Course"
                : "Add Course"}
            </button>

            {editingCourse && (
              <button
                type="button"
                className="secondary"
                onClick={clearForm}
              >
                Cancel Edit
              </button>
            )}
          </form>
        </section>

        <section>
          <h2>All Courses</h2>

          <div className="course-grid">
            {courses.map((course) => (
              <div className="course-card" key={course.id}>
                <h3>{course.title}</h3>

                <p>{course.description}</p>

                <p>
                  <strong>Instructor:</strong>{" "}
                  {course.instructor}
                </p>

                <button onClick={() => startEdit(course)}>
                  Edit
                </button>

                <button
                  className="danger"
                  onClick={() => deleteCourse(course.id)}
                >
                  Delete
                </button>
              </div>
            ))}
          </div>
        </section>
      </main>
    </div>
  );
}

// NAVBAR
function Nav({ user, logout }) {
  return (
    <nav>
      <h2>Course Management</h2>

      <div>
        <span>Welcome, {user.name}</span>
        <button onClick={logout}>Logout</button>
      </div>
    </nav>
  );
}

export default App;