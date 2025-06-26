# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## プロジェクト概要

このプロジェクトは「UnityCleanArchitectureTodo」という名前で、UnityアプリでClean Architectureパターンを実装したTodoアプリのサンプルです。

## プロジェクト構成

- **プロジェクト名**: UnityCleanArchitectureTodo
- **Unity プロジェクトパス**: `UnityCleanArchitechtureTodo/`
- **ビルド**: Unityエディタからビルド

## 主要な依存関係とライブラリ

### Unity Package Manager (.upm)
- **R3** (v1.3.0): リアクティブプログラミング用ライブラリ
- **UniTask** (v2.5.10): 非同期処理とタスク管理
- **VContainer** (v1.16.9): 依存性注入コンテナ（Clean Architectureの実装に使用）
- **NuGet For Unity** (v4.4.0): .NET NuGetパッケージをUnityで使用するためのツール

### NuGet パッケージ
- **Microsoft.Bcl.AsyncInterfaces** (v6.0.0): 非同期インターフェース
- **Microsoft.Bcl.TimeProvider** (v8.0.0): 時間プロバイダー
- **R3** (v1.3.0): リアクティブ拡張ライブラリ
- **System.ComponentModel.Annotations** (v5.0.0): データ注釈
- **System.Runtime.CompilerServices.Unsafe** (v6.0.0): アンセーフコード関連
- **System.Threading.Channels** (v8.0.0): チャンネルベースの非同期処理

## アーキテクチャパターン

このプロジェクトはClean Architectureパターンを採用しており、以下の特徴があります：

- **VContainer**: 依存性注入による疎結合の実現
- **UniTask**: 非同期処理の効率的な管理
- **R3**: リアクティブプログラミングによるデータフローの管理

## 開発環境

- Unity Editor (バージョンは ProjectSettings/ProjectVersion.txt で確認)
- .NET標準ライブラリ対応
- NuGet For Unity によるライブラリ管理

## 注意事項

現在、このプロジェクトには具体的なC#スクリプトファイルが存在しないため、今後の開発でClean Architectureの各レイヤー（Presentation、Application、Domain、Infrastructure）を実装する予定と思われます。

## ファイル構成

- `Assets/`: Unity アセットフォルダ
- `Assets/Scenes/`: シーンファイル
- `Assets/Packages/`: NuGetパッケージ
- `Packages/`: Unity Package Manager パッケージ設定
- `ProjectSettings/`: プロジェクト設定ファイル