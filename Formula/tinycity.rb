class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "2.0.5"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.0.5/tinycity-v2.0.5-osx-arm64"
      sha256 "ebe7fc56b6375ca81c72d392b958d4b9f77f97ca1c05d6c00e0d01b50d91dcb0"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.0.5/tinycity-v2.0.5-osx-x64"
      sha256 "ebd203308ddd97db076cf2b4d3f189a31dd94ff25130d505d8dbb4b82c4d4768"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v2.0.5/tinycity-v2.0.5-linux-x64"
    sha256 "2fb19babca4f20f8aed99c464fdd7d85a150e12a7ea48e3cf1f11dee87f3e8b6"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v2.0.5-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v2.0.5-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v2.0.5-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end







