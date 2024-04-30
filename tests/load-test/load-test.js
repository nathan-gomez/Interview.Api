import http from "k6/http";
import { check, sleep } from "k6";

const ApiKey = "e63fc33856450a0b4a33cbe99e1c6d30f11c17d1e8d483ad5ae28388015de725";
const urls = {
	login: "https://interview-api.srv-desa.nfg-tech.com/v1/users/login",
	getClients: "https://interview-api.srv-desa.nfg-tech.com/v1/clients",
};

export const options = {
	stages: [
		{ duration: "5s", target: 10 },
		{ duration: "10m", target: 20 },
		{ duration: "30s", target: 0 },
	],
};

export function setup() {
	const payload = JSON.stringify({
		username: "Admin",
		password: "12345678",
	});

	const params = {
		headers: {
			"Content-Type": "application/json",
			"x-api-key": ApiKey,
		},
	};

	let response = http.post(urls.login, payload, params);

	check(response, {
		"is status 200": (r) => r.status === 200,
		"login response has cookie 'session_token'": (r) =>
			r.cookies.session_token !== null,
	});

	return response.cookies.session_token[0].value;
}

export default function (data) {
	const params = {
		headers: {
			"Content-Type": "application/json",
			"x-api-key": ApiKey,
		},
		cookies: {
			session_token: data,
		},
	};

	let response = http.get(urls.getClients, params);

	check(response, {
		"is status 200": (r) => r.status === 200,
	});
	sleep(1);
}
