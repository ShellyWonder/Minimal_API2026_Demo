global using System;
global using System.Text;
global using System.ComponentModel.DataAnnotations; 
global using System.Text.Json.Serialization;

global using System.Security.Claims;
global using Microsoft.AspNetCore.Identity.Data;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity.UI.Services;

global using MinimalAPI2026Demo.Data;
global using MinimalAPI2026Demo.Middleware;
global using MinimalAPI2026Demo.Services;
global using MinimalAPI2026Demo.Services.Interfaces;

global using MinimalAPI2026Demo.Models;
global using MinimalAPI2026Demo.Filters;
global using MinimalAPI2026Demo.Authentication;
global using MinimalAPI2026Demo.Endpoints.Home;
global using MinimalAPI2026Demo.Endpoints.CustomIdentityEndpoints;
global using MinimalAPI2026Demo.Endpoints.CustomIdentityEndpoints.Models;
global using MinimalAPI2026Demo.Endpoints.Sites;
global using MinimalAPI2026Demo.Endpoints.Artifacts;
global using MinimalAPI2026Demo.Endpoints.CatalogRecords;
global using MinimalAPI2026Demo.Extensions;
global using MinimalAPI2026Demo.Authorization;

global using ThePlatoProject.Contracts.Requests;
global using ThePlatoProject.Contracts.Responses;
global using ThePlatoProject.Contracts.Enums;
global using ThePlatoProject.Contracts.Authorization;
global using ThePlatoProject.Contracts.Authentication;

//global using ThePlatoProject.Contracts.DTOs;
